import Link from "next/link";
import { cookies } from "next/headers";
import { logoutAction } from "@/app/login/actions";

export default async function Navbar() {
  const isLoggedIn = (await cookies()).has(`${process.env.COOKIE_NAME}`);

  return (
    <nav className="fixed top-0 z-50 w-full border-b border-black/[.08] bg-white/80 backdrop-blur-md dark:border-white/[.145] dark:bg-black/80">
      <div className="mx-auto flex h-16 max-w-7xl items-center justify-between px-6 sm:px-16">
        <Link 
          href="/home" 
          className="text-lg font-bold tracking-tight text-black dark:text-zinc-50 transition-all duration-200 hover:opacity-70 active:scale-[0.98] active:opacity-80"
        >
          {process.env.APP_NAME}
        </Link>

        <div className="flex items-center gap-6">
          {!isLoggedIn ? (
            <div className="flex items-center gap-2">
              <Link 
                href="/login" 
                className="flex h-9 items-center justify-center rounded-full border border-black/[.08] px-4 text-sm font-medium text-black transition-all hover:bg-zinc-50 dark:border-white/[.145] dark:text-white dark:hover:bg-zinc-900 active:scale-95"
              >
                Log in
              </Link>
              <Link
                href="/signup"
                className="flex h-9 items-center justify-center rounded-full bg-black px-4 text-sm font-medium text-white transition-all hover:bg-[#383838] dark:bg-white dark:text-black dark:hover:bg-[#ccc] active:scale-95"
              >
                Sign up
              </Link>
            </div>
          ) : (
            <div className="flex items-center gap-4">
              <Link 
                href="/dashboard" 
                className="text-sm font-medium text-zinc-600 transition-all hover:text-black dark:text-zinc-400 dark:hover:text-white active:scale-95 active:opacity-70"
              >
                Dashboard
              </Link>
              <form action={logoutAction}>
                <button
                  type="submit"
                  className="flex h-9 items-center justify-center rounded-full border border-black/[.08] px-4 text-sm font-medium text-black transition-all hover:bg-zinc-50 dark:border-white/[.145] dark:text-white dark:hover:bg-zinc-900 active:scale-95"
                >
                  Log out
                </button>
              </form>
            </div>
          )}
        </div>
      </div>
    </nav>
  );
}
