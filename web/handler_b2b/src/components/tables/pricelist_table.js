import { Stack, Table } from "react-bootstrap";
import PropTypes from "prop-types";

/**
 * Create a table that holds pricelist items. If there are not items empty element shall be returned.
 * @component
 * @param {object} props Component props
 * @param {Array<{partnumber: string, itemName: string, qty: Number, price: Number}>} props.items Array with object that contain items.
 * @param {string} props.currency Shortcut of currency name.
 * @return {JSX.Element} Table element
 */
function PricelistTable({ items, currency }) {
  if (items.length === 0) {
    return <></>;
  }
  return (
    <Table className="text-start overflow-x-scroll align-middle" bordered>
      <thead>
        <tr>
          <th className="top-left-rounded">
            <p className="mb-0">Product</p>
          </th>
          <th>Qty</th>
          <th className="top-right-rounded">Price</th>
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
              <td className="no-wrap">
                <Stack direction="horizontal">
                  <span className="me-auto">
                    {parseFloat(Math.round(value.price * 100) / 100).toFixed(2)}
                  </span>
                  <span className="ps-2">{currency}</span>
                </Stack>
              </td>
            </tr>
          );
        })}
      </tbody>
      <thead>
        <tr>
          <th className="bottom-both-rounded" colSpan={3}></th>
        </tr>
      </thead>
    </Table>
  );
}

PricelistTable.propTypes = {
  items: PropTypes.array.isRequired,
  currency: PropTypes.string.isRequired,
};

export default PricelistTable;
