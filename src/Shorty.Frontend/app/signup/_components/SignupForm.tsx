'use client'

import { useActionState, useEffect, useState } from "react";
import { signupAction } from "../actions";

export default function ShortenForm() {
  const [state, formAction, isPending] = useActionState(signupAction, null);

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [errors, setErrors] = useState<Record<string, string[] | undefined>>({});

  useEffect(() => {
    if (state?.errors) {
      setErrors(state.errors);
    }
  }, [state])

  const handleEmailChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setEmail(e.target.value);
    setErrors((prev) => ({ ...prev, email: undefined, _form: undefined }));
  }

  const handlePasswordChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setPassword(e.target.value);
    setErrors((prev) => ({ ...prev, password: undefined, _form: undefined }));
  }

  const handleConfirmPasswordChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setConfirmPassword(e.target.value);
    setErrors((prev) => ({ ...prev, confirmPassword: undefined, _form: undefined }));
  }

  return (
    <form action={formAction} noValidate autoComplete="off" className="flex w-full flex-col gap-12">
      <div className="flex w-full flex-col gap-6">
        <div className="flex flex-col gap-1.5">
          <div className={`flex h-auto py-3 min-h-[72px] w-full flex-col justify-center rounded-full border border-solid px-8 bg-white dark:bg-zinc-950 focus-within:border-black dark:focus-within:border-white transition-colors gap-1 ${errors?.email ? 'border-red-500' : 'border-black/[.08] dark:border-white/[.145]'}`}>
            <label className="text-[10px] font-bold uppercase tracking-[0.1em] text-zinc-500">
              Email
            </label>
            <input 
              name="email"
              value={email}
              onChange={handleEmailChange}
              maxLength={255}
              placeholder="Enter your email"
              className="w-full bg-transparent text-base outline-none text-zinc-950 dark:text-zinc-50 placeholder:text-zinc-500"
            />
          </div>
          {errors?.email && (
            <p className="text-xs font-medium text-red-500 ml-8">{errors.email[0]}</p>
          )}
        </div>

        <div className="flex flex-col gap-1.5">
          <div className={`flex h-auto py-3 min-h-[72px] w-full flex-col justify-center rounded-full border border-solid px-8 bg-white dark:bg-zinc-950 focus-within:border-black dark:focus-within:border-white transition-colors gap-1 ${errors?.password ? 'border-red-500' : 'border-black/[.08] dark:border-white/[.145]'}`}>
            <label className="text-[10px] font-bold uppercase tracking-[0.1em] text-zinc-500">
              Password
            </label>
            <input 
              name="password"
              type="password"
              value={password}
              onChange={handlePasswordChange}
              maxLength={64}
              placeholder="Enter your password"
              className="w-full bg-transparent text-base outline-none text-zinc-950 dark:text-zinc-50 placeholder:text-zinc-500"
            />
          </div>
          {errors?.password && (
            <p className="text-xs font-medium text-red-500 ml-8">{errors.password[0]}</p>
          )}
        </div>

        <div className="flex flex-col gap-1.5">
          <div className={`flex h-auto py-3 min-h-[72px] w-full flex-col justify-center rounded-full border border-solid px-8 bg-white dark:bg-zinc-950 focus-within:border-black dark:focus-within:border-white transition-colors gap-1 ${errors?.confirmPassword ? 'border-red-500' : 'border-black/[.08] dark:border-white/[.145]'}`}>
            <label className="text-[10px] font-bold uppercase tracking-[0.1em] text-zinc-500">
              Confirm Password
            </label>
            <input 
              name="confirmPassword"
              type="password"
              value={confirmPassword}
              onChange={handleConfirmPasswordChange}
              maxLength={64}
              placeholder="Confirm your password"
              className="w-full bg-transparent text-base outline-none text-zinc-950 dark:text-zinc-50 placeholder:text-zinc-500"
            />
          </div>
          {errors?.confirmPassword && (
            <p className="text-xs font-medium text-red-500 ml-8">{errors.confirmPassword[0]}</p>
          )}
        </div>
      </div>

      <div className="flex flex-col gap-4 text-base font-medium sm:flex-row">
        <button
          type="submit"
          disabled={isPending}
          className="flex h-12 w-full items-center justify-center gap-2 rounded-full bg-black px-5 text-white transition-all hover:bg-zinc-800 dark:bg-zinc-50 dark:text-black dark:hover:bg-zinc-200 md:w-[158px] disabled:opacity-50 disabled:cursor-not-allowed active:scale-95"
        >
          {isPending ? "Signing up..." : "Sign up"}
        </button>
      </div>

      {errors?._form && (
        <p className="text-sm font-medium text-red-500 animate-in fade-in slide-in-from-left-2">
          {errors._form[0]}
        </p>
      )}
    </form>
  );
}
