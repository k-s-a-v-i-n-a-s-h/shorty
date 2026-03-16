use sqlx::sqlite::{SqliteConnectOptions, SqlitePool};
use std::{env, str::FromStr, time::Duration};

pub async fn create_pool() -> SqlitePool {
    let db_url = env::var("DATABASE_URL").expect("DATABASE_URL not found");

    let timeout_ms: u64 = env::var("DATABASE_BUSY_TIMEOUT_MS")
        .expect("DATABASE_BUSY_TIMEOUT_MS not found")
        .parse()
        .expect("DATABASE_BUSY_TIMEOUT_MS must be a number");

    let connection_options = SqliteConnectOptions::from_str(&db_url)
        .expect("Invalid DATABASE_URL format")
        .journal_mode(sqlx::sqlite::SqliteJournalMode::Wal)
        .synchronous(sqlx::sqlite::SqliteSynchronous::Normal)
        .busy_timeout(Duration::from_millis(timeout_ms));

    return SqlitePool::connect_with(connection_options)
        .await
        .expect("Failed to connect to SQLite");
}
