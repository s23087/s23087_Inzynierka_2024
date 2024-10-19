import { Stack, Table } from "react-bootstrap";
import PropTypes from "prop-types";

function CreditNoteTable({ creditItems, currency }) {
  if (creditItems.length === 0) {
    return <></>;
  }
  let sumPrice = 0;
  if (creditItems.length > 0) {
    creditItems.forEach((element) => {
      sumPrice += element.price * element.qty;
    });
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
      <tbody key={creditItems}>
        {Object.values(creditItems).map((value, key) => {
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
          <th className="bottom-left-rounded" colSpan={2}>
            Sum:
          </th>
          <th className="bottom-right-rounded">
            <Stack direction="horizontal">
              <span className="me-auto">{parseFloat(sumPrice).toFixed(2)}</span>
              <span className="ps-2">{currency}</span>
            </Stack>
          </th>
        </tr>
      </thead>
    </Table>
  );
}

CreditNoteTable.propTypes = {
  creditItems: PropTypes.array.isRequired,
  currency: PropTypes.string.isRequired,
};

export default CreditNoteTable;
