import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import Link from "next/link";
import LinkItem from "./_components/LinkItem";
import { loadDashboard } from "./actions";
import { Metadata } from "next";


export const metadata: Metadata = {
  title: { absolute: `Dashboard` },
  description: `Manage, and track your links.`,
}

export default async function DashboardPage() {
  const isLoggedIn = (await cookies()).has(`${process.env.COOKIE_NAME}`);
  if (!isLoggedIn) {
    redirect("/login");
  }

  const data = await loadDashboard();

  return (
    <div className="flex h-screen flex-col items-center bg-zinc-50 font-sans dark:bg-black overflow-hidden">
      <main className="flex h-full w-full max-w-7xl flex-col gap-12 p-8 md:p-24">
        <div className="flex shrink-0 flex-col gap-6 sm:flex-row sm:items-center sm:justify-between">
          <div className="flex flex-col gap-1">
            <h1 className="text-3xl font-semibold tracking-tight text-black dark:text-zinc-50">Dashboard</h1>
            <p className="text-[10px] text-zinc-500 uppercase tracking-[0.2em] font-bold">
              {data.links?.length || 0} Links
            </p>
          </div>
          <Link href="/home" className="flex h-11 items-center justify-center rounded-full bg-black px-8 text-sm font-medium text-white transition-all hover:bg-zinc-800 dark:bg-zinc-50 dark:text-black dark:hover:bg-zinc-200 active:scale-95">
            Shorten URL
          </Link>
        </div>

        <div className="flex flex-1 flex-col gap-4 overflow-y-auto pb-12 pr-1 [&::-webkit-scrollbar]:hidden [-ms-overflow-style:none] [scrollbar-width:none]">
          {data.links?.map((link: any) => (
            <LinkItem key={link.id} link={link} />
          ))}
        </div>
      </main>
    </div>
  );
}
