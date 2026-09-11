import {
  Alert,
  Box,
  Divider,
  MenuItem,
  Paper,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { BarChart } from "@mui/x-charts/BarChart";
import { PieChart } from "@mui/x-charts/PieChart";
import { useState } from "react";
import { formatMoney } from "@shared/lib/format";
import { orderStatusLabel } from "@features/orders";
import {
  groupByLabel,
  percentageOf,
  type GroupBy,
  type OrderDashboard,
  type Slice,
} from "../model/dashboard";

interface Props {
  dashboard: OrderDashboard;
}

/**
 * Monta as fatias do agrupamento escolhido.
 *
 * Todas contam **pedidos**, e nao valor. Contagem atravessa moedas sem
 * problema; valor nao -- somar reais com dolares num unico grafico produziria
 * uma fatia sem significado. O valor aparece separado, por moeda.
 */
function slicesOf(dashboard: OrderDashboard, groupBy: GroupBy): Slice[] {
  switch (groupBy) {
    case "customer":
      return dashboard.byCustomer.map((row) => ({
        label: row.customer,
        value: row.orderCount,
      }));
    case "status":
      return dashboard.byStatus.map((row) => ({
        label: orderStatusLabel[row.status] ?? row.status,
        value: row.orderCount,
      }));
    case "currency":
      return dashboard.totalsByCurrency.map((row) => ({
        label: row.currency,
        value: row.orderCount,
      }));
  }
}

export default function OrdersCharts({ dashboard }: Props) {
  const [groupBy, setGroupBy] = useState<GroupBy>("customer");

  const slices = slicesOf(dashboard, groupBy);
  const total = slices.reduce((sum, slice) => sum + slice.value, 0);

  if (dashboard.totalOrders === 0) {
    return (
      <Alert severity="info" sx={{ mb: 3 }}>
        Nenhum pedido ainda. Os gráficos aparecem assim que houver o primeiro.
      </Alert>
    );
  }

  return (
    <Stack spacing={3} sx={{ mb: 3 }}>
      <Paper sx={{ p: 2 }}>
        <Stack
          direction="row"
          justifyContent="space-between"
          alignItems="center"
          sx={{ mb: 1 }}
        >
          <Typography variant="subtitle1">Participação nos pedidos</Typography>

          <TextField
            select
            size="small"
            label="Agrupar por"
            value={groupBy}
            onChange={(event) => setGroupBy(event.target.value as GroupBy)}
            sx={{ width: 180 }}
          >
            {(Object.keys(groupByLabel) as GroupBy[]).map((option) => (
              <MenuItem key={option} value={option}>
                {groupByLabel[option]}
              </MenuItem>
            ))}
          </TextField>
        </Stack>

        <Divider sx={{ mb: 2 }} />

        <Stack direction="row" spacing={3} alignItems="center" flexWrap="wrap" useFlexGap>
          <PieChart
            series={[
              {
                data: slices.map((slice, index) => ({
                  id: index,
                  value: slice.value,
                  label: slice.label,
                })),
                highlightScope: { fade: "global", highlight: "item" },
                // O rotulo fica na legenda; dentro da fatia fica ilegivel
                // quando ha muitos grupos pequenos.
                arcLabel: (item) => percentageOf(item.value, total),
                arcLabelMinAngle: 25,
              },
            ]}
            height={260}
            width={420}
          />

          <Box sx={{ minWidth: 220 }}>
            {slices.map((slice) => (
              <Stack
                key={slice.label}
                direction="row"
                justifyContent="space-between"
                sx={{ py: 0.5 }}
              >
                <Typography variant="body2" color="text.secondary">
                  {slice.label}
                </Typography>
                <Typography variant="body2">
                  {slice.value} ({percentageOf(slice.value, total)})
                </Typography>
              </Stack>
            ))}
          </Box>
        </Stack>
      </Paper>

      <Paper sx={{ p: 2 }}>
        <Typography variant="subtitle1" sx={{ mb: 1 }}>
          Pedidos por data de abertura
        </Typography>
        <Divider sx={{ mb: 2 }} />

        <BarChart
          xAxis={[
            {
              scaleType: "band",
              data: dashboard.byDate.map((row) =>
                new Date(`${row.date}T00:00:00`).toLocaleDateString("pt-BR", {
                  day: "2-digit",
                  month: "2-digit",
                })
              ),
            },
          ]}
          series={[
            {
              data: dashboard.byDate.map((row) => row.orderCount),
              label: "Pedidos",
            },
          ]}
          height={260}
        />
      </Paper>

      <Paper sx={{ p: 2 }}>
        <Typography variant="subtitle1" sx={{ mb: 1 }}>
          Valor total por moeda
        </Typography>
        <Divider sx={{ mb: 2 }} />

        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
          Separado por moeda de propósito: um total que soma reais com dólares não
          significa nada.
        </Typography>

        <Stack spacing={1} sx={{ maxWidth: 360 }}>
          {dashboard.totalsByCurrency.map((row) => (
            <Stack key={row.currency} direction="row" justifyContent="space-between">
              <Typography variant="body2" color="text.secondary">
                {row.currency} ({row.orderCount} pedido{row.orderCount === 1 ? "" : "s"})
              </Typography>
              <Typography variant="body2">{formatMoney(row.total, row.currency)}</Typography>
            </Stack>
          ))}
        </Stack>
      </Paper>
    </Stack>
  );
}
