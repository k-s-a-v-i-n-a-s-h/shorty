'use client'

import { useActionState, useEffect, useState } from "react";
import { createLinkAction } from "../actions";
import DomainSelector from "./DomainSelector";

export default function ShortenForm({ isLoggedIn, availableDomains, defaultDomain }: { isLoggedIn: boolean, availableDomains: string[], defaultDomain: string }) {
  const [state, formAction, isPending] = useActionState(createLinkAction, null);

  const [url, setUrl] = useState("");
  const [alias, setAlias] = useState("");
  const [errors, setErrors] = useState<Record<string, string[] | undefined>>({});

  const [copied, setCopied] = useState(false);

  const handleCopy = () => {
    if (state?.link) {
      navigator.clipboard.writeText(state.link);
      setCopied(true);

      setTimeout(() => setCopied(false), 2000);
    }
  }

  useEffect(() => {
    if (state?.errors) {
      setErrors(state.errors);
    }
  }, [state])

  const handleUrlChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setUrl(e.target.value);
    setErrors((prev) => ({ ...prev, url: undefined, _form: undefined }));
  }

  const handleAliasChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setAlias(e.target.value);
    setErrors((prev) => ({ ...prev, alias: undefined, _form: undefined }));
  }

  return (
    <form action={formAction} noValidate autoComplete="off" className="flex w-full flex-col gap-12">
      <div className="flex w-full flex-col gap-6">
        <div className="flex flex-col gap-1.5">
          <div className={`flex h-auto py-3 min-h-[72px] w-full flex-col justify-center rounded-full border border-solid px-8 bg-white dark:bg-zinc-950 focus-within:border-black dark:focus-within:border-white transition-colors gap-1 ${errors?.url ? 'border-red-500' : 'border-black/[.08] dark:border-white/[.145]'}`}>
            <label className="text-[10px] font-bold uppercase tracking-[0.1em] text-zinc-500">
              URL
            </label>
            <input 
              name="url"
              value={url}
              onChange={handleUrlChange}
              readOnly={state?.success}
              maxLength={2048}
              placeholder="https://example.com"
              className="w-full bg-transparent text-base outline-none text-zinc-950 dark:text-zinc-50 placeholder:text-zinc-500"
            />
          </div>
          {errors?.url && (
            <p className="text-xs font-medium text-red-500 ml-8">{errors.url[0]}</p>
          )}
        </div>
        
        {isLoggedIn && (
          <div className="flex flex-col gap-1.5">
            <div className={`flex h-auto py-3 min-h-[72px] flex-1 rounded-full border border-solid px-8 bg-white dark:bg-zinc-950 focus-within:border-black dark:focus-within:border-white transition-colors ${errors?.alias ? 'border-red-500' : 'border-black/[.08] dark:border-white/[.145]'}`}>
              <DomainSelector availableDomains={availableDomains} defaultDomain={defaultDomain} isLoggedIn={isLoggedIn} setErrors={setErrors}/>
              <div className="flex items-center mx-4 text-zinc-300 dark:text-zinc-700 text-xl font-light">/</div>
              <div className="flex flex-col justify-center flex-1 gap-1">
                <label className="text-[10px] font-bold uppercase tracking-[0.1em] text-zinc-500">Alias (Optional)</label>
                <input 
                  name="alias"
                  value={alias}
                  onChange={handleAliasChange}
                  readOnly={state?.success}
                  maxLength={255}
                  placeholder="enter-alias"
                  className="w-full bg-transparent text-base outline-none text-zinc-950 dark:text-zinc-50 placeholder:text-zinc-500"
                />
              </div>
            </div>
            {(errors?.domain || errors?.alias) && (
              <p className="text-xs font-medium text-red-500 ml-8">{errors.domain?.[0] || errors.alias?.[0]}</p>
            )}
          </div>
        )}

        {state?.success && state.link && (
          <div className="flex flex-col gap-3 pt-6 animate-in fade-in slide-in-from-bottom-4 duration-500">
            <div className="flex h-auto py-3 min-h-[72px] w-full flex-row items-center rounded-full border border-blue-500/30 bg-blue-50/50 px-8 dark:bg-blue-900/10">

              <div className="flex flex-col justify-center flex-1 gap-1">
                <label className="text-[10px] font-bold uppercase tracking-[0.1em] text-blue-600 dark:text-blue-400 select-none">
                  Link
                </label>
                <input
                  readOnly
                  value={state.link}
                  className="w-full bg-transparent text-base font-bold text-zinc-950 dark:text-zinc-50 outline-none truncate"
                />
              </div>

              <button
                type="button"
                onClick={handleCopy}
                className={`ml-4 h-10 px-6 rounded-full text-xs font-bold transition-all duration-200 shrink-0 ${copied ? "bg-green-600 text-white" : "bg-blue-600 text-white hover:bg-blue-700"}`}
              >
                {copied ? "Copied!" : "Copy"}
              </button>
            </div>
          </div>
        )}
      </div>

      <div className="flex flex-col gap-4 text-base font-medium sm:flex-row">
        {state?.success ? (
          <button
            type="button"
            onClick={() => window.location.reload()}
            className="flex h-12 w-full items-center justify-center gap-2 rounded-full bg-foreground px-5 text-background transition-all duration-200 hover:bg-[#383838] dark:hover:bg-[#ccc] active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed md:w-[200px]"
          >
            Shorten Another
          </button>
        ) : (
          <button
            type="submit"
            disabled={isPending}
            className="flex h-12 w-full items-center justify-center gap-2 rounded-full bg-black px-5 text-white transition-all hover:bg-zinc-800 dark:bg-zinc-50 dark:text-black dark:hover:bg-zinc-200 md:w-[158px] disabled:opacity-50 disabled:cursor-not-allowed active:scale-95"
          >
            {isPending ? "Shortening..." : "Shorten"}
          </button>
        )}
      </div>

      {errors?._form && (
        <p className="text-sm font-medium text-red-500 animate-in fade-in slide-in-from-left-2">
          {errors._form[0]}
        </p>
      )}
    </form>
  );
}
