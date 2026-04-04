"""Pure interval overlap check (same rule as SQL in booking repository)."""


def intervals_overlap(start1, end1, start2, end2) -> bool:
    return start1 < end2 and start2 < end1
