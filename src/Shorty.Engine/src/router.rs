use crate::repository;
use crate::state::AppState;
use axum::extract::{Path, State};
use axum::http::StatusCode;
use axum::response::{IntoResponse, Redirect};
use axum::{Router, routing};
use std::sync::Arc;

pub fn create_router(state: Arc<AppState>) -> Router {
    Router::new()
        .route("/api/links/:slug", routing::get(redirect_handler))
        .with_state(state)
}

async fn redirect_handler(
    Path(slug): Path<String>,
    State(state): State<Arc<AppState>>,
) -> impl IntoResponse {
    match repository::fetch_link_by_slug(&state.db, &slug).await {
        Ok(Some(full_url)) => Redirect::to(&full_url).into_response(),
        Ok(None) => StatusCode::NOT_FOUND.into_response(),
        Err(_) => StatusCode::INTERNAL_SERVER_ERROR.into_response(),
    }
}
