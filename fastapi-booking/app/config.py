from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    model_config = SettingsConfigDict(env_file=".env", env_file_encoding="utf-8", extra="ignore")

    database_url: str = "sqlite:///./booking.db"
    jwt_secret: str = "SuperSecretKeyForJwtMustBeAtLeast32CharactersLong!"
    jwt_algorithm: str = "HS256"
    jwt_expire_minutes: int = 120
    max_bookings_per_day: int = 5

    bootstrap_admin_enabled: bool = False
    bootstrap_admin_email: str = "admin@local"
    bootstrap_admin_password: str = ""


settings = Settings()
