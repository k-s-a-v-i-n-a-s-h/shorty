use chrono::{DateTime, Utc};
use moka::future::Cache;
use moka::policy::Expiry;
use std::time::{Duration, Instant};

#[derive(Clone)]
pub struct CachedLink {
    pub full_url: String,
    pub expires_at: DateTime<Utc>,
}

pub fn create_cache() -> Cache<String, CachedLink> {
    let max_bytes: u64 = std::env::var("CACHE_MAX_BYTES")
        .expect("CACHE_MAX_BYTES not found")
        .parse()
        .expect("CACHE_MAX_BYTES must be a number");

    Cache::builder()
        .max_capacity(max_bytes)
        .weigher(|key: &String, value: &CachedLink| (key.len() + value.full_url.len() + 128) as u32)
        .expire_after(LinkExpiry)
        .build()
}

struct LinkExpiry;

impl Expiry<String, CachedLink> for LinkExpiry {
    fn expire_after_create(
        &self,
        _key: &String,
        value: &CachedLink,
        _created_at: Instant,
    ) -> Option<Duration> {
        let now = Utc::now();
        if value.expires_at > now {
            Some(Duration::from_secs(
                (value.expires_at - now).num_seconds() as u64
            ))
        } else {
            Some(Duration::ZERO)
        }
    }
}
