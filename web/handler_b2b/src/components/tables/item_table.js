import { Stack, Table } from "react-bootstrap";
import PropTypes from "prop-types";

function ItemTable({ restInfo, isOurWarehouse }) {
  if (restInfo.outsideItemInfos.length === 0 && !isOurWarehouse) {
    return <></>;
  }
  if (restInfo.ownedItemInfos.length === 0 && isOurWarehouse) {
    return <></>;
  }
  let qtySum = 0;
  let avgPrice = 0;
  if (restInfo.ownedItemInfos.length > 0) {
    restInfo.ownedItemInfos.forEach((element) => {
      qtySum = qtySum + element.qty;
      avgPrice = avgPrice + element.price;
    });
    avgPrice =
      Math.round((avgPrice / restInfo.ownedItemInfos.length) * 100) / 100;
  }
  return (
    <Table className="text-start overflow-x-scroll align-middle" bordered>
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
      <tbody key={restInfo}>
        {isOurWarehouse
          ? Object.values(restInfo.ownedItemInfos).map((value, key) => {
              return (
                <tr key={key}>
                  <td>
                    <p className="mb-0">
                      {value.organizationName + "\n" + value.invoiceNumber}
                    </p>
                  </td>
                  <td className="text-center">{value.qty}</td>
                  <td className="no-wrap">
                    <Stack direction="horizontal">
                      <span className="me-auto">
                        {parseFloat(
                          Math.round(value.price * 100) / 100,
                        ).toFixed(2)}
                      </span>
                      <span className="ps-2">{value.currency}</span>
                    </Stack>
                  </td>
                </tr>
              );
            })
          : Object.values(restInfo.outsideItemInfos).map((value, key) => {
              return (
                <tr key={key}>
                  <td>
                    <p className="mb-0">{value.organizationName}</p>
                  </td>
                  <td className="text-center">{value.qty}</td>
                  <td className="no-wrap">
                    {value.price + " " + value.currency}
                  </td>
                </tr>
              );
            })}
      </tbody>
      <thead>
        <tr>
          <th className="bottom-left-rounded">
            {isOurWarehouse ? "Sum & Avg price" : ""}
          </th>
          <th className="text-center">{isOurWarehouse ? qtySum : ""}</th>
          <th className="bottom-right-rounded">
            {isOurWarehouse ? avgPrice : ""}
          </th>
        </tr>
      </thead>
    </Table>
  );
}

ItemTable.propTypes = {
  restInfo: PropTypes.object.isRequired,
  isOurWarehouse: PropTypes.bool.isRequired
};

export default ItemTable;
