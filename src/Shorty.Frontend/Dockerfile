# Stage 1: Install dependencies
FROM node:20-slim AS deps
RUN apt-get update && apt-get install -y libc6 && rm -rf /var/lib/apt/lists/*
WORKDIR /app

# Copy ONLY the package.json (Ignore the lockfile for now to bypass the npm bug)
COPY package.json ./

# This will re-resolve dependencies for Linux and download the correct 
# tailwind-oxide and lightningcss binaries automatically.
RUN npm install

# Stage 2: Rebuild the source code
FROM node:20-slim AS builder
WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY . .

ENV NEXT_TELEMETRY_DISABLED=1

RUN npm run build

# Stage 3: Production runner
FROM node:20-slim AS runner
WORKDIR /app

ENV NODE_ENV=production
ENV NEXT_TELEMETRY_DISABLED=1

RUN groupadd --system --gid 1001 nodejs
RUN useradd --system --uid 1001 nextjs

COPY --from=builder /app/public ./public
RUN mkdir .next && chown nextjs:nodejs .next

COPY --from=builder --chown=nextjs:nodejs /app/.next/standalone ./
COPY --from=builder --chown=nextjs:nodejs /app/.next/static ./.next/static

USER nextjs

EXPOSE 8000
ENV PORT=8000
ENV HOSTNAME="0.0.0.0"

CMD ["node", "server.js"]
