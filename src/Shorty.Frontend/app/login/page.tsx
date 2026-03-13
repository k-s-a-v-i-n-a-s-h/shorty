import { Metadata } from "next"
import { cookies } from "next/headers";
import LoginForm from "./_components/LoginForm"
import { redirect } from "next/navigation";

export const metadata: Metadata = {
  title: "Log in",
  description: `Log in to your ${process.env.APP_NAME} account to create, manage, and track your shortened links.`,
}

export default async function LoginPage() {
  const isLoggedIn = (await cookies()).has(`${process.env.COOKIE_NAME}`);
  if (isLoggedIn) {
    redirect("/home");
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-zinc-50 font-sans dark:bg-black">
      <main className="flex w-full max-w-3xl flex-col items-center gap-16 justify-center py-32 px-16 bg-white dark:bg-black sm:items-start">
        <div className="flex flex-col items-center gap-6 text-center sm:items-start sm:text-left">
          <h1 className="max-w-xs text-3xl font-semibold leading-10 tracking-tight text-black dark:text-zinc-50">
            Access custom aliases and domains
          </h1>
        </div>
        <LoginForm />
      </main>
    </div>
  );
}
