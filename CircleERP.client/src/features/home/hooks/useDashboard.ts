import { useEffect, useState } from "react";
import { toApiError } from "@shared/api/problemDetails";
import { dashboardApi } from "../api/dashboardApi";
import type { OrderDashboard } from "../model/dashboard";

interface UseDashboardResult {
  dashboard: OrderDashboard | null;
  isLoading: boolean;
  loadError: string | null;
}

export function useDashboard(): UseDashboardResult {
  const [dashboard, setDashboard] = useState<OrderDashboard | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [loadError, setLoadError] = useState<string | null>(null);

  useEffect(() => {
    const controller = new AbortController();

    dashboardApi
      .read(controller.signal)
      .then(setDashboard)
      .catch((error) => {
        if (controller.signal.aborted) return;
        setLoadError(toApiError(error).message);
      })
      .finally(() => {
        if (!controller.signal.aborted) setIsLoading(false);
      });

    return () => controller.abort();
  }, []);

  return { dashboard, isLoading, loadError };
}
