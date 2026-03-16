'use client'

import { useState } from 'react'
import Link from 'next/link'

interface DomainSelectorProps {
  availableDomains: string[];
  defaultDomain: string;
  isLoggedIn: boolean;
  setErrors: React.Dispatch<React.SetStateAction<Record<string, string[] | undefined>>>;
}

export default function DomainSelector({ availableDomains, defaultDomain, isLoggedIn, setErrors }: DomainSelectorProps) {
  const [isOpen, setIsOpen] = useState(false)
  const [selectedDomain, setSelectedDomain] = useState(defaultDomain)

  const handleDomainChange = (domain: string) => {
    setSelectedDomain(domain);
    setIsOpen(false);

    setErrors((prev) => ({ ...prev, domain: undefined, _form: undefined }));
  };

  availableDomains = [defaultDomain, ...availableDomains];

  return (
    <div className="group relative flex flex-col justify-center min-w-[160px] gap-1 cursor-pointer" onClick={() => setIsOpen(!isOpen)}>
      <input type='hidden' name='domain' value={selectedDomain}/>
      
      <label className="text-[10px] font-bold uppercase tracking-[0.1em] text-zinc-500 select-none">
        Domain
      </label>

      <div className="flex items-center text-base font-medium text-zinc-950 dark:text-zinc-50">
        {selectedDomain}
        <span className={`ml-2 text-[8px] text-zinc-400 transition-transform duration-200 ${isOpen ? 'rotate-180' : ''}`}>
          ▼
        </span>
      </div>

      {isOpen && (
        <>
          <div className="fixed inset-0 z-40" onClick={() => setIsOpen(false)} />
          
          <div className="absolute left-[-12px] top-[110%] z-50 min-w-[220px] rounded-xl border border-black/[.08] bg-white p-1.5 shadow-xl animate-in fade-in zoom-in-95 dark:border-white/[.145] dark:bg-zinc-900">
            {availableDomains.map((domain) => (
              <div
                key={domain}
                onClick={(e) => {
                  e.stopPropagation();
                  handleDomainChange(domain);
                }}
                className="px-3 py-2 text-sm rounded-lg hover:bg-zinc-100 dark:hover:bg-zinc-800 text-zinc-900 dark:text-zinc-100 transition-colors"
              >
                {domain}
              </div>
            ))}

            {isLoggedIn && (
              <>
                <div className="my-1 h-px bg-black/[.05] dark:bg-white/[.10]" />
                <Link
                  href="/domains/add"
                  className="block px-3 py-2 text-sm font-semibold text-blue-600 rounded-lg hover:bg-blue-50 dark:hover:bg-blue-950/30 transition-colors"
                >
                  Add Domain
                </Link>
              </>
            )}
          </div>
        </>
      )}
    </div>
  )
}
