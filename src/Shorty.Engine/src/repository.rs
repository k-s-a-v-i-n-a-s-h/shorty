use sqlx::sqlite::SqlitePool;

pub async fn fetch_link_by_slug(
    pool: &SqlitePool,
    slug: &str,
) -> Result<Option<String>, sqlx::Error> {
    let link = sqlx::query_scalar::<_, String>(
        "SELECT Url FROM Links WHERE Slug = ? AND Status = 1 AND ExpiresAt > CURRENT_TIMESTAMP",
    )
    .bind(&slug)
    .fetch_optional(pool)
    .await?;

    Ok(link)
}
