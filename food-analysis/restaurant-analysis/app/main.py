import logging
from contextlib import asynccontextmanager
from typing import AsyncIterator

from fastapi import FastAPI

from app.api.routes.analyses import router as analyses_router
from app.core.config import get_settings
from app.db.session import engine

settings = get_settings()
logging.basicConfig(level=settings.log_level.upper())


@asynccontextmanager
async def lifespan(_: FastAPI) -> AsyncIterator[None]:
    yield
    engine.dispose()


app = FastAPI(
    title=settings.app_name,
    description="Create, save, and retrieve restaurant profile analyses.",
    version="1.0.0",
    lifespan=lifespan,
)
app.include_router(analyses_router, prefix=settings.api_v1_prefix)


@app.get("/health", tags=["health"])
def health() -> dict[str, str]:
    return {"status": "ok"}
