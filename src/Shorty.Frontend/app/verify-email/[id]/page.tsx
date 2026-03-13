import { Metadata } from "next"
import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import VerifyForm from "./_components/VerifyForm";
import { HIDDEN_ROBOTS } from "@/lib/metadata";

export const metadata: Metadata = {
  title: "Verifying your email...",
  description: `Confirm your email to access your ${process.env.APP_NAME} account.`,
  robots: HIDDEN_ROBOTS,
}

type Props = {
    params: Promise<{ id: string }>,
    searchParams: Promise<{ token: string }>,
}

export default async function LoginPage({ params, searchParams }: Props) {
  const isLoggedIn = (await cookies()).has(`${process.env.COOKIE_NAME}`);
  if (isLoggedIn) {
    redirect("/home");
  }

  const { id } = await params;
  const { token } = await searchParams;

  return (
    <div className="flex min-h-screen items-center justify-center bg-zinc-50 font-sans dark:bg-black">
      <main className="flex w-full max-w-3xl flex-col items-center gap-16 justify-center py-32 px-16 bg-white dark:bg-black sm:items-start">
        <VerifyForm id={id} token={token}/>
      </main>
    </div>
  );
}
