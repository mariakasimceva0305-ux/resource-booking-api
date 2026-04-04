from datetime import datetime, timezone

from app.intervals import intervals_overlap


def test_overlapping_intervals():
    a1 = datetime(2026, 4, 4, 10, 0, tzinfo=timezone.utc)
    a2 = datetime(2026, 4, 4, 11, 0, tzinfo=timezone.utc)
    b1 = datetime(2026, 4, 4, 10, 30, tzinfo=timezone.utc)
    b2 = datetime(2026, 4, 4, 12, 0, tzinfo=timezone.utc)
    assert intervals_overlap(a1, a2, b1, b2) is True


def test_touching_boundary_no_overlap():
    a1 = datetime(2026, 4, 4, 10, 0, tzinfo=timezone.utc)
    a2 = datetime(2026, 4, 4, 11, 0, tzinfo=timezone.utc)
    b1 = a2
    b2 = datetime(2026, 4, 4, 12, 0, tzinfo=timezone.utc)
    assert intervals_overlap(a1, a2, b1, b2) is False
