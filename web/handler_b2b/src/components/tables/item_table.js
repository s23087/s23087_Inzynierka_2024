import { Table } from "react-bootstrap";

function ItemTable({ restInfo, isOurWarehouse }) {
  if (restInfo.outsideItemInfos.length == 0 && !isOurWarehouse) {
    return <></>;
  }
  if (restInfo.ownedItemInfos.length == 0 && isOurWarehouse) {
    return <></>;
  }
  let qtySum = 0;
  let avgPrice = 0;
  if (restInfo.ownedItemInfos.length > 0) {
    restInfo.ownedItemInfos.array.forEach((element) => {
      qtySum = qtySum + element.qty;
      avgPrice = avgPrice + element.price;
    });
    avgPrice = avgPrice / restInfo.ownedItemInfos.length;
  }
  return (
    <Table className="text-start" bordered>
      <thead>
        <tr>
          <th className="top-left-rounded">
            <p className="mb-0">
              {isOurWarehouse ? "Source & Invoice" : "Source"}
            </p>
          </th>
          <th>Qty</th>
          <th className="top-right-rounded">Price</th>
        </tr>
      </thead>
      <tbody>
        {isOurWarehouse
          ? Object.entries(restInfo).map((key, value) => {
              return (<tr key={key}>
                <td>
                  <p className="mb-0">
                    {value.organizationName + "\n" + value.invoiceNumber}
                  </p>
                </td>
                <td>{value.qty}</td>
                <td>{value.price + " " + value.currency}</td>
              </tr>);
            })
          : Object.entries(restInfo).map((key, value) => {
              return (<tr key={key}>
                <td>
                  <p className="mb-0">{value.organizationName}</p>
                </td>
                <td>{value.qty}</td>
                <td>{value.price + " " + value.currency}</td>
              </tr>);
            })}
      </tbody>
      <thead>
        <tr>
          <th className="bottom-left-rounded">
            {isOurWarehouse ? "Sum & Avg price" : ""}
          </th>
          <th>{isOurWarehouse ? qtySum : ""}</th>
          <th className="bottom-right-rounded">
            {isOurWarehouse ? avgPrice : ""}
          </th>
        </tr>
      </thead>
    </Table>
  );
}

export default ItemTable;
