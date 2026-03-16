'use server'

import { z } from "zod";
import { cookies } from "next/headers";

const schema = z.object({
    url: z.string()
        .min(1, "URL is required")
        .url("Invalid URL"),
    alias: z.preprocess(
        (val) => (val === "" || val === null ? undefined : val), 
        z.string()
            .min(5, "Alias must be at least 5 characters")
            .regex(/^[a-zA-Z0-9-]+$/, "Alias can only contain letters, numbers, and dashes")
            .optional()
    ),
    domain: z.preprocess(
        (val) => (val === "" || val === null ? undefined : val), 
        z.string()
            .min(1, "Domain is required")
            .optional()
    ),
});

export async function createLinkAction(prevState: any, formData: FormData) {
    const validation = schema.safeParse({
        url: formData.get("url"),
        alias: formData.get("alias"),
        domain: formData.get("domain"),
    });

    console.log(formData.get("domain"));

    if (!validation.success) {
        return { errors: validation.error.flatten().fieldErrors };
    }
    
    const token = (await cookies()).get(`${process.env.COOKIE_NAME}`)?.value;
    let res: Response

    try {
        res = await fetch(`${process.env.MANAGEMENT_BASE_URL}${process.env.SHORTEN_PATH}`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify(validation.data),
        });
    } catch (err) {
        console.error("fetch failed:", err);
        return { errors: { _form: [ "Something went wrong. Kindly try again." ] } };
    }

    const data = await res.json().catch(() => ({}));
    console.log(data);

    if (!res.ok) {
        if (res.status == 422 && data.errors) {
            return { errors: data.errors };
        }

        if (res.status == 409 && data.detail) {
            return { errors: { alias: [ data.detail ] } }
        }

        if (res.status == 500 && data.detail) {
            return { errors: { _form: [ data.detail ] } }
        }
    }

    return { success: true, link: data.shortUrl };
}
