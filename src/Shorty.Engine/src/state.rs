use sqlx::sqlite::SqlitePool;

pub struct AppState {
    pub db: SqlitePool,
}
