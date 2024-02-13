import EnhancedTable from "components/Table/EnhancedTable";
import { useState } from "react";
import { ICurrency } from "shared";



export default function listIndex (){
  const [currency, setCurrency] = useState<ICurrency[]>([]);
  return (
    <EnhancedTable
      data={currency}
      setData={setCurrency}
      // headCells={headCells}
      deleteData={() => {}}
    />
  )
}