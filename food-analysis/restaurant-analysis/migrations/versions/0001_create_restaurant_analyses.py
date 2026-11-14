"""Create the restaurant analysis history table."""

from typing import Sequence, Union

from alembic import op
import sqlalchemy as sa
from sqlalchemy.dialects import postgresql

revision: str = "0001_restaurant_analyses"
down_revision: Union[str, None] = None
branch_labels: Union[str, Sequence[str], None] = None
depends_on: Union[str, Sequence[str], None] = None


def upgrade() -> None:
    op.create_table(
        "RestaurantAnalyses",
        sa.Column("Id", sa.Integer(), autoincrement=True, nullable=False),
        sa.Column("RestaurantId", sa.Integer(), nullable=False),
        sa.Column("RestaurantName", sa.String(length=200), nullable=False),
        sa.Column("AnalysisType", sa.String(length=80), nullable=False),
        sa.Column("Summary", sa.String(length=1000), nullable=False),
        sa.Column("Details", postgresql.JSONB(astext_type=sa.Text()), nullable=False),
        sa.Column("CreatedAt", sa.DateTime(timezone=True), server_default=sa.text("now()"), nullable=False),
        sa.ForeignKeyConstraint(["RestaurantId"], ["Restaurants.Id"], ondelete="CASCADE"),
        sa.PrimaryKeyConstraint("Id"),
    )
    op.create_index(
        "IX_RestaurantAnalyses_RestaurantId_CreatedAt",
        "RestaurantAnalyses",
        ["RestaurantId", "CreatedAt"],
    )


def downgrade() -> None:
    op.drop_index("IX_RestaurantAnalyses_RestaurantId_CreatedAt", table_name="RestaurantAnalyses")
    op.drop_table("RestaurantAnalyses")
