'use client'

import { useActionState, useState } from "react";
import { updateLinkAction, deleteLinkAction } from "../actions";

export default function LinkItem({ link }: { link: any }) {
  const [_, updateAction, isUpdating] = useActionState(updateLinkAction, null);
  const [__, delAction, isDeleting] = useActionState(deleteLinkAction, null);
  const [copied, setCopied] = useState(false);

  const isActive = link.status === 'Active';

  const handleCopy = async () => {
    try {
      await navigator.clipboard.writeText(link.shortUrl);
      setCopied(true);
      setTimeout(() => setCopied(false), 1000);
    } catch (err) {
      console.error("Failed to copy!", err);
    }
  };

  return (
    <div className="flex min-h-[110px] w-full items-center justify-between rounded-full border border-black/[.08] bg-white px-16 transition-all hover:border-black dark:border-white/[.145] dark:bg-zinc-950 dark:hover:border-white hover:shadow-sm">
      <div className="flex flex-[3] flex-col gap-0.5 overflow-hidden pr-12">
        <label className="text-[10px] font-bold uppercase tracking-[0.2em] text-zinc-500">
          {copied ? "Copied!" : "Link / URL"}
        </label>
        <button 
          onClick={handleCopy} 
          title="Click to copy"
          className="w-full truncate text-left text-lg font-semibold text-zinc-950 transition-opacity hover:opacity-70 dark:text-zinc-50"
        >
          {link.shortUrl}
        </button>
        <a 
          href={link.url} 
          target="_blank" 
          rel="noopener noreferrer" 
          title="Click to open"
          className="w-full truncate text-xs text-zinc-400 transition-colors hover:text-black dark:hover:text-white"
        >
          {link.url}
        </a>
      </div>

      <div className="hidden max-w-[120px] flex-1 flex-col gap-0.5 sm:flex">
        <label className="text-[10px] font-bold uppercase tracking-[0.2em] text-zinc-500">Status</label>
        <span className={`text-sm font-bold ${isActive ? 'text-zinc-950 dark:text-zinc-50' : 'text-zinc-400'}`}>
          {link.status}
        </span>
      </div>

      <div className="hidden max-w-[220px] flex-1 flex-col gap-0.5 lg:flex">
        <label className="text-[10px] font-bold uppercase tracking-[0.2em] text-zinc-500">Expiry</label>
        <span className={`text-xs font-semibold ${link.expiresAt ? 'text-zinc-950 dark:text-zinc-50' : 'text-zinc-400'}`}>
          {link.expiresAt ? formatAestheticDate(link.expiresAt) : 'Never'}
        </span>
      </div>

      <div className="flex items-center justify-end gap-10 pl-10 shrink-0">
        <form action={updateAction} className="flex items-center">
          <input type="hidden" name="id" value={link.id} />
          <input type="hidden" name="isEnabled" value={(!isActive).toString()} />
          
          <button 
            type="submit" 
            disabled={isUpdating} 
            className="relative flex h-8 w-[85px] items-center justify-center text-[10px] font-black uppercase tracking-[0.3em] text-zinc-400 transition-all duration-200 hover:text-black dark:hover:text-white disabled:opacity-50 active:scale-95"
          >
            <span className={`transition-opacity duration-200 ${isUpdating ? "opacity-0" : "opacity-100"}`}>
              {isActive ? "Disable" : "Enable"}
            </span>
            
            {isUpdating && (
              <span className="absolute inset-0 flex items-center justify-center animate-pulse">
                •
              </span>
            )}
          </button>
        </form>

        <form action={delAction} className="flex items-center">
          <input type="hidden" name="id" value={link.id} />
          <button 
            type="submit" 
            disabled={isDeleting} 
            onClick={(e) => !confirm("Delete link?") && e.preventDefault()}
            className="flex items-center text-zinc-400 transition-all duration-200 hover:text-red-500 disabled:opacity-30 active:scale-95"
          >
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" className="block">
              <path d="M3 6h18m-2 0v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6m3 0V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2"/>
            </svg>
          </button>
        </form>
      </div>
    </div>
  );
}

function formatAestheticDate(dateString: string) {
  if (!dateString) return null;
  const date = new Date(dateString);
  const day = date.getUTCDate();
  const months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
  const getSuffix = (d: number) => {
    if (d > 3 && d < 21) return 'th';
    switch (d % 10) { case 1: return "st"; case 2: return "nd"; case 3: return "rd"; default: return "th"; }
  };
  const pad = (n: number) => n.toString().padStart(2, '0');
  return `${day}${getSuffix(day)} ${months[date.getUTCMonth()]} ${date.getUTCFullYear()}, ${pad(date.getUTCHours())}:${pad(date.getUTCMinutes())}:${pad(date.getUTCSeconds())}`;
}
