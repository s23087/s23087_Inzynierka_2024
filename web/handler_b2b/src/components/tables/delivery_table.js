import { Table } from "react-bootstrap";
import PropTypes from "prop-types";

/**
 * Create a table that holds delivery items. If there are not items empty element shall be returned.
 * @component
 * @param {object} props Component props
 * @param {Array<{partnumber: string, itemName: string, qty: number}>} props.items Array with object that contain items.
 * @return {JSX.Element} Table element
 */
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
        {Object.values(items).map((value) => {
          return (
            <tr key={value}>
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

DeliveryTable.propTypes = {
  items: PropTypes.array.isRequired,
};

export default DeliveryTable;
