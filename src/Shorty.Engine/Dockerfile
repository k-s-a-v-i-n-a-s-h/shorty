FROM debian:bookworm-slim AS base
WORKDIR /app

RUN useradd -m appUser && \
    mkdir -p /data/sqlite && \
    chown -R appUser:appUser /app /data
USER appUser

FROM rustlang/rust:nightly AS builder
WORKDIR /src

COPY Cargo.toml Cargo.lock ./
RUN mkdir -p src && echo "fn main() {}" > src/main.rs && cargo build --release
RUN rm -f target/release/deps/shorty_engine*

COPY . .
RUN cargo build --release

FROM base AS final
COPY --from=builder --chown=appUser:appUser /src/target/release/shorty_engine .
ENTRYPOINT ["./shorty_engine"]
