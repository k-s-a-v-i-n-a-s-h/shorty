'use server'

import { cookies } from "next/headers";
import { parse } from "set-cookie-parser";
import { z } from "zod";

const schema = z.object ({
    userId: z.string().uuid({ message: "Invalid Id" }),
    token: z.string()
        .regex(/^[A-Za-z0-9_-]+$/, { message: "Invalid Base64Url" })
        .length(43, "HMAC-SHA256 must be 43 characters"),
})

export async function verifyEmailAction(prevState: any, { id, token }: { id: string, token: string }) {
    const validation = schema.safeParse({
        userId: id,
        token,
    });

    if (!validation.success) {
        console.log("validation failed: ", JSON.stringify(validation.error.flatten().fieldErrors));
        return { invalid: true };
    }

    let res: Response;
    
    try {
        res = await fetch(`${process.env.MANAGEMENT_BASE_URL}${process.env.VERIFY_PATH}`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(validation.data),
        });
    } catch (err) {
        console.error("fetch failed: ", err);
        return { isError: true };
    }

    const data = await res.json().catch(() => ({}));

    if (res.status == 404 || res.status == 422 || res.status == 400) {
        return { invalid: true };
    }

    if (res.status == 409) {
        return { alreadyVerified: true }
    }

    if (res.status == 500 && data.detail) {
        console.error("server error: ", data.detail);
        return { isError: true };
    }

    var cookieStore = await cookies();
    parse(res.headers.getSetCookie()).forEach(({ name, value, ...options }) => {
        cookieStore.set(name, value, options as any);
    });

    return { success: true }
}
