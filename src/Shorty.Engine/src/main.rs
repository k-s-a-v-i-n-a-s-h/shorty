use crate::state::AppState;
use dotenvy::dotenv;
use std::sync::Arc;

mod db;
mod repository;
mod router;
mod state;

#[tokio::main]
async fn main() {
    dotenv().ok();

    let shared_state = Arc::new(AppState {
        db: db::create_pool().await,
    });

    let port = std::env::var("APP_PORT").expect("APP_PORT not found");
    let addr = format!("0.0.0.0:{}", port);

    let app: axum::Router = router::create_router(shared_state);
    let listener = tokio::net::TcpListener::bind(addr).await.unwrap();

    axum::serve(listener, app)
        .with_graceful_shutdown(async {
            tokio::signal::ctrl_c()
                .await
                .expect("failed to install CTRL+C handler");
        })
        .await
        .unwrap();
}
