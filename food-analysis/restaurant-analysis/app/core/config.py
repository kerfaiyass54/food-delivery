from functools import lru_cache

from pydantic import Field, PostgresDsn, field_validator
from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    model_config = SettingsConfigDict(
        env_file=".env",
        env_file_encoding="utf-8",
        case_sensitive=False,
        extra="ignore",
    )

    app_name: str = "Restaurant Analysis API"
    api_v1_prefix: str = "/api/v1"
    log_level: str = "INFO"
    database_url: PostgresDsn = Field(validation_alias="DATABASE_URL")

    @field_validator("api_v1_prefix")
    @classmethod
    def normalize_api_prefix(cls, value: str) -> str:
        prefix = "/" + value.strip("/")
        if prefix == "/":
            raise ValueError("API_V1_PREFIX must not be empty")
        return prefix


@lru_cache
def get_settings() -> Settings:
    return Settings()
