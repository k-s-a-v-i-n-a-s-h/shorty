use crate::cache::CachedLink;
use moka::future::Cache;
use sqlx::sqlite::SqlitePool;

pub struct AppState {
    pub db: SqlitePool,
    pub cache: Cache<String, CachedLink>,
}
