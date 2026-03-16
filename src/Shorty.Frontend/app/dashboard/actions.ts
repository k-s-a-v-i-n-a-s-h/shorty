'use server'

import { cookies } from "next/headers";
import { revalidatePath } from "next/cache";

const BASE = process.env.MANAGEMENT_BASE_URL;
const DASHBOARD_PATH = process.env.DASHBOARD_PATH;
const UPDATE_PATH = process.env.UPDATE_LINK_PATH; 
const DELETE_PATH = process.env.DELETE_LINK_PATH; 
const COOKIE_NAME = process.env.COOKIE_NAME!;

export async function loadDashboard() {
  const token = (await cookies()).get(COOKIE_NAME)?.value;
  const url = `${BASE}${DASHBOARD_PATH}`;

  let res: Response;

  try {
    res = await fetch(url, {
      headers: { "Authorization": `Bearer ${token}` },
      cache: 'no-store'
    });
  } catch (err) {
    console.error("fetch failed: ", err);
    throw err;
  }

  if (!res.ok) {
    throw new Error(`couldn't load the dashboard: ${res.status}`);
  }

  return res.json().catch(() => ({}));
}

export async function updateLinkAction(prevState: any, formData: FormData) {
  const id = formData.get("id") as string;
  const isEnabled = formData.get("isEnabled") === "true";
  const token = (await cookies()).get(COOKIE_NAME)?.value;
  const url = `${BASE}${UPDATE_PATH?.replace("{id}", id)}`;

  let res: Response;

  try {
    res = await fetch(url, {
      method: "PATCH",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
      },
      body: JSON.stringify({ isEnabled })
    });
  } catch (err) {
    console.error("fetch failed: ", err);
    throw err;
  }

  if (!res.ok) return { errors: await res.json().catch(() => ({})) };
  revalidatePath("/dashboard");
}

export async function deleteLinkAction(prevState: any, formData: FormData) {
  const id = formData.get("id") as string;
  const token = (await cookies()).get(COOKIE_NAME)?.value;
  const url = `${BASE}${DELETE_PATH?.replace("{id}", id)}`;

  try {
    await fetch(url, {
      method: "DELETE",
      headers: { "Authorization": `Bearer ${token}` }
    });
  } catch (err) {
    console.error("fetch failed: ", err);
    throw err;
  }

  revalidatePath("/dashboard");
}
