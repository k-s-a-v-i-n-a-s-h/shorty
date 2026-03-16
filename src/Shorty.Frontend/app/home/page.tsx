import { Metadata } from "next";
import { cookies } from "next/headers";
import ShortenForm from "./_components/ShortenForm";

export const metadata: Metadata = {
  title: { absolute: `${process.env.APP_NAME}: Modern, fast URL shortener` },
  description: `Create, manage, and track your links with ${process.env.APP_NAME}.`,
}

export default async function HomePage() {
  const isLoggedIn = (await cookies()).has(`${process.env.COOKIE_NAME}`);
  const domains: string[] = [];

  return (
    <div className="flex min-h-screen items-center justify-center bg-zinc-50 font-sans dark:bg-black">
      <main className="flex w-full max-w-3xl flex-col items-center gap-16 justify-center py-32 px-16 bg-white dark:bg-black sm:items-start">
        <div className="flex flex-col items-center gap-6 text-center sm:items-start sm:text-left">
          <h1 className="max-w-xs text-3xl font-semibold leading-10 tracking-tight text-black dark:text-zinc-50">
            Create links that look professional
          </h1>
          {!isLoggedIn && (<p className="max-w-md text-lg leading-8 text-zinc-600 dark:text-zinc-400">
            Need custom aliases and domains? {" "}
            <a
              href="/login"
              className="font-medium text-zinc-950 dark:text-zinc-50 hover:opacity-80 transition-opacity"
            >
              Log in
            </a>{" "}
            or {" "}
            <a
              href="/signup"
              className="font-medium text-zinc-950 dark:text-zinc-50 hover:opacity-80 transition-opacity"
            >
              create an account
            </a>{""}
            .
          </p>)}
        </div>
        <ShortenForm isLoggedIn={isLoggedIn} availableDomains={domains} defaultDomain={`${process.env.APP_BASE_URL}`}/>
      </main>
    </div>
  );
}
