import { SortDirection } from "@shared/types/sorting";

/**
 * Ordena preservando a ordem original entre elementos equivalentes.
 */
export function stableSort<T>(
  items: readonly T[],
  comparator: (a: T, b: T) => number
): T[] {
  if (!Array.isArray(items)) {
    return [];
  }

  return items
    .map((item, index) => [item, index] as [T, number])
    .sort((a, b) => comparator(a[0], b[0]) || a[1] - b[1])
    .map(([item]) => item);
}

function descendingComparator<T>(a: T, b: T, orderBy: keyof T): number {
  const left = a[orderBy];
  const right = b[orderBy];

  // Campos opcionais (um simbolo ausente, por exemplo) vao para o fim.
  if (left == null && right == null) return 0;
  if (left == null) return 1;
  if (right == null) return -1;

  if (right < left) return -1;
  if (right > left) return 1;
  return 0;
}

export function getComparator<T>(
  direction: SortDirection,
  orderBy: keyof T
): (a: T, b: T) => number {
  return direction === "desc"
    ? (a, b) => descendingComparator(a, b, orderBy)
    : (a, b) => -descendingComparator(a, b, orderBy);
}
