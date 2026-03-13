use chrono::{DateTime, Utc};
use sqlx::sqlite::SqlitePool;

pub async fn fetch_link_by_slug(
    pool: &SqlitePool,
    slug: &str,
) -> Result<Option<(String, DateTime<Utc>)>, sqlx::Error> {
    let slug = slug.to_string();

    let link = sqlx::query_as::<_, (String, DateTime<Utc>)>(
        "SELECT Url, ExpiresAt FROM Links WHERE Slug = ? AND Status = 1",
    )
    .bind(&slug)
    .fetch_optional(pool)
    .await?;

    if link.is_some() {
        let pool_clone = pool.clone();

        tokio::spawn(async move {
            let _ = sqlx::query(
                "UPDATE Links SET TotalClicks = TotalClicks + 1 WHERE Slug = ? AND Status = 1",
            )
            .bind(slug)
            .execute(&pool_clone)
            .await;
        });
    }

    Ok(link)
}
