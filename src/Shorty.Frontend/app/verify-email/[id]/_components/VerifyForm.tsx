'use client'

import { startTransition, useActionState, useEffect, useRef } from "react";
import { verifyEmailAction } from "../actions";

export default function VerifyForm({ id, token }: { id: string, token: string }) {
  const [state, formAction, isPending] = useActionState(verifyEmailAction, null);
  const hasCalledAction = useRef(false);

  useEffect(() => {
    if (!hasCalledAction.current && id && token) {
      hasCalledAction.current = true;
      startTransition(() => {
        formAction({ id, token });
      });
    }
  }, [id, token, formAction]);

  useEffect(() => {
    if (state?.success) {
      const timer = setTimeout(() => {
        window.location.href = '/home';
      }, 5000);
      return () => clearTimeout(timer);
    }
  }, [state?.success]);

  return (
    <div className="flex flex-col items-center gap-6 text-center sm:items-start sm:text-left">
      {state?.invalid && (
        <><h1 className="max-w-xs text-3xl font-semibold leading-10 tracking-tight text-black dark:text-zinc-50">
          Invalid or expired link
        </h1>
        <p className="max-w-md text-lg leading-8 text-zinc-600 dark:text-zinc-400">
          <a
            href="/verify-email/resend"
            className="font-medium text-zinc-950 dark:text-zinc-50 hover:opacity-80 transition-opacity"
          >
            Resend verification email
          </a>{" "}
          to finish setting up.
        </p></>
      )}

      {state?.alreadyVerified && (
        <><h1 className="max-w-xs text-3xl font-semibold leading-10 tracking-tight text-black dark:text-zinc-50">
          Used link
        </h1>
        <p className="max-w-md text-lg leading-8 text-zinc-600 dark:text-zinc-400">
          Your email is already verified.{" "}
          <a
            href="/login"
            className="font-medium text-zinc-950 dark:text-zinc-50 hover:opacity-80 transition-opacity"
          >
            Log in
          </a>{" "}
          to get started.
        </p></>
      )}

      {state?.isError && (
        <><h1 className="max-w-xs text-3xl font-semibold leading-10 tracking-tight text-black dark:text-zinc-50">
          Something went wrong
        </h1>
        <p className="max-w-md text-lg leading-8 text-zinc-600 dark:text-zinc-400">
          We're having trouble verifying your email. Please try again later.
        </p></>
      )}

      {state?.success && (
        <><h1 className="max-w-xs text-3xl font-semibold leading-10 tracking-tight text-black dark:text-zinc-50">
          Email verified
        </h1>
        <p className="max-w-md text-lg leading-8 text-zinc-600 dark:text-zinc-400">
          Your account is now active.
        </p>
        <p className="max-w-md text-lg leading-8 text-zinc-600 dark:text-zinc-400">
          <i>Redirecting you in 5 seconds...</i>
        </p></>
      )}
    </div> 
  );
}
