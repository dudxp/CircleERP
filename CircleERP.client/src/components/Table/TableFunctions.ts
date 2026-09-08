import { Order } from "@shared/mainConfig";

export function stableSort<T>(
  array: readonly T[],
  comparator: (a: T, b: T) => number
): T[] {
  if (!Array.isArray(array)) {
    return [];
  }

  const stabilized = array.map((element, index) => [element, index] as [T, number]);

  stabilized.sort((a, b) => {
    const order = comparator(a[0], b[0]);

    if (order !== 0) return order;

    return a[1] - b[1];
  });

  return stabilized.map(([element]) => element);
}

export function descendingComparator<T>(a: T, b: T, orderBy: keyof T): number {
  if (b[orderBy] < a[orderBy]) {
    return -1;
  }
  if (b[orderBy] > a[orderBy]) {
    return 1;
  }
  return 0;
}

export function getComparator<T>(
  order: Order,
  orderBy: keyof T
): (a: T, b: T) => number {
  return order === "desc"
    ? (a, b) => descendingComparator(a, b, orderBy)
    : (a, b) => -descendingComparator(a, b, orderBy);
}
