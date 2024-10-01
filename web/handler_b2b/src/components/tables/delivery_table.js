import { Table } from "react-bootstrap";

function DeliveryTable({ items }) {
  if (items.length === 0) {
    return <></>;
  }
  let sumQty = 0;
  if (items.length > 0) {
    items.forEach((element) => {
      sumQty += element.qty;
    });
  }
  return (
    <Table className="text-start overflow-x-scroll align-middle" bordered>
      <thead>
        <tr>
          <th className="top-left-rounded">
            <p className="mb-0">Product</p>
          </th>
          <th className="top-right-rounded px-3">Qty</th>
        </tr>
      </thead>
      <tbody key={items}>
        {Object.values(items).map((value, key) => {
          return (
            <tr key={key}>
              <td>
                <p className="mb-0 break-spaces">
                  {value.partnumber + "\n" + value.itemName}
                </p>
              </td>
              <td className="text-center">{value.qty}</td>
            </tr>
          );
        })}
      </tbody>
      <thead>
        <tr>
          <th className="bottom-left-rounded">Sum:</th>
          <th className="bottom-right-rounded text-center">{sumQty}</th>
        </tr>
      </thead>
    </Table>
  );
}

export default DeliveryTable;
