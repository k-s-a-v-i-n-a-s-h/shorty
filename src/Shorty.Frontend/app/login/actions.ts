'use server'

import { z } from "zod";
import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import { parse } from "set-cookie-parser";

const schema = z.object({
    email: z.string()
        .min(1, "Email is required")
        .email("Invalid email"),
    password: z.string()
        .min(1, "Password is required")
        .min(8, "Password must be at least 8 characters")
        .max(64, "Password is too long")
});

export async function loginAction(prevState: any, formData: FormData) {
    const validation = schema.safeParse({
        email: formData.get("email"),
        password: formData.get("password"),
    });

    if (!validation.success) {
        return { errors: validation.error.flatten().fieldErrors };
    }
    
    let res: Response

    try {
        res = await fetch(`${process.env.MANAGEMENT_BASE_URL}${process.env.LOGIN_PATH}`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
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

        if (res.status == 404 && data.detail) {
            return { errors: { email: [ data.detail ] } };
        }

        if (res.status == 403 && data.detail) {
            return { errors: { email: [ data.detail ] }, unverified: true };
        }

        if (res.status == 401 && data.detail) {
            return { errors: { password: [ data.detail ] } }
        }

        if (res.status == 500 && data.detail) {
            return { errors: { _form: [ data.detail ] } }
        }

        if (res.status == 429) {
            return { errors: { _form: [ "Too many requests. Try again later." ] } }
        }
    }

    var cookieStore = await cookies();
    parse(res.headers.getSetCookie()).forEach(({ name, value, ...options }) => {
        cookieStore.set(name, value, options as any);
    });

    redirect("/home");
}


export async function logoutAction() {
    (await cookies()).delete(`${process.env.COOKIE_NAME}`);

    redirect("/home");
}
