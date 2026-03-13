'use server'

import { z } from "zod";
import { redirect } from "next/navigation";

const schema = z.object({
    email: z.string()
        .min(1, "Email is required")
        .email("Invalid email"),
    password: z.string()
        .min(1, "Password is required")
        .min(8, "Password must be at least 8 characters")
        .max(64, "Password must be 64 characters or fewer"),
    confirmPassword: z.string()
})
.refine((data) => {
    return data.password === data.confirmPassword
}, {
    message: "Passwords do not match",
    path: ["confirmPassword"],
});

export async function signupAction(prevState: any, formData: FormData) {
    const validation = schema.safeParse({
        email: formData.get("email"),
        password: formData.get("password"),
        confirmPassword: formData.get("confirmPassword"),
    });

    if (!validation.success) {
        return { errors: validation.error.flatten().fieldErrors };
    }
    
    let res: Response

    try {
        res = await fetch(`${process.env.MANAGEMENT_BASE_URL}${process.env.SIGNUP_PATH}`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                email: validation.data.email,
                password: validation.data.password,
            }),
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
            return { errors: { email: [ data.detail ] } };
        }

        if (res.status == 500 && data.detail) {
            return { errors: { _form: [ data.detail ] } }
        }

        if (res.status == 429) {
            return { errors: { _form: [ "Too many requests. Try again later." ] } }
        }
    }

    redirect("/verify-email");
}
