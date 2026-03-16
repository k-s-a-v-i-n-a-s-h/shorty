import { Metadata } from 'next';

export const BASE_ROBOTS: Metadata['robots'] = {
  index: true,
  follow: true,
  googleBot: {
    index: true,
    follow: true,
  }
}

export const HIDDEN_ROBOTS: Metadata['robots'] = {
  index: false,
  follow: false,
  nocache: true,
  noarchive: true,
  googleBot: {
    index: false,
    follow: false,
    noarchive: true,
  },
};
