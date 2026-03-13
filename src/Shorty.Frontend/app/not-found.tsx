import { Metadata } from "next"
import { HIDDEN_ROBOTS } from "@/lib/metadata";

export const metadata: Metadata = {
  title: `Page not found`,
  description: `The page you were looking for wasn't found`,
  robots: HIDDEN_ROBOTS,
}

export default function NotFound() {
  return (
    <div className="flex min-h-screen items-center justify-center bg-zinc-50 font-sans dark:bg-black">
      <main className="flex w-full max-w-3xl flex-col items-center gap-16 justify-center py-32 px-16 bg-white dark:bg-black sm:items-start">
        <div className="flex flex-col items-center gap-6 text-center sm:items-start sm:text-left">
          <h1 className="max-w-xs text-3xl font-semibold leading-10 tracking-tight text-black dark:text-zinc-50">
            Page not found
          </h1>
          <p className="max-w-md text-lg leading-8 text-zinc-600 dark:text-zinc-400">
            The link you followed may be broken, or the page may have been removed.
          </p>
        </div>
      </main>
    </div>
  )
}
