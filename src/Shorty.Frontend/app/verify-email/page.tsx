import { Metadata } from "next"
import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import { HIDDEN_ROBOTS } from "@/lib/metadata";

export const metadata: Metadata = {
  title: "Verify your email",
  description: `Verify your ${process.env.APP_NAME} account to start creating, managing, and tracking your shortened links.`,
  robots: HIDDEN_ROBOTS,
}

export default async function VerifyEmailPage() {
  const isLoggedIn = (await cookies()).has(`${process.env.COOKIE_NAME}`);
  if (isLoggedIn) {
    redirect("/home");
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-zinc-50 font-sans dark:bg-black">
      <main className="flex w-full max-w-3xl flex-col items-center gap-16 justify-center py-32 px-16 bg-white dark:bg-black sm:items-start">
        <div className="flex flex-col items-center gap-6 text-center sm:items-start sm:text-left">
          <h1 className="max-w-xs text-3xl font-semibold leading-10 tracking-tight text-black dark:text-zinc-50">
            Check your inbox to finish setting up
          </h1>
          <p className="max-w-md text-lg leading-8 text-zinc-600 dark:text-zinc-400">
            We sent a verification link to your email.
          </p>
          <p className="max-w-md text-lg leading-8 text-zinc-600 dark:text-zinc-400">
            <i>Don't see it? Check your spam folder or {" "}
              <a
                href="/verify-email/resend"
                className="font-medium text-zinc-950 dark:text-zinc-50 hover:opacity-80 transition-opacity"
              >
                resend the email
              </a>
              .
            </i>
          </p>
        </div>
      </main>
    </div>
  );
}
