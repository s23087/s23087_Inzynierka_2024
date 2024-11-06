import PropTypes from "prop-types";
import { Container, Stack, Button } from "react-bootstrap";

/**
 * Create element allowing to chose sort order.
 * @component
 * @param {object} props
 * @param {boolean} props.isAsc useState variable. If true then ascending button is disabled, otherwise descending button is disabled
 * @param {Function} props.setIsAsc useState function from isAsc variable
 * @return {JSX.Element} Container element
 */
function SortOrderComponent({ isAsc, setIsAsc }) {
  return (
    <Container className="px-1 ms-0 pb-3">
      <p className="mb-1 blue-main-text">Sort order</p>
      <Stack
        direction="horizontal"
        className="align-items-center"
        style={{ maxWidth: "329px" }}
      >
        <Button
          className="w-100 me-2"
          disabled={isAsc}
          onClick={() => setIsAsc(true)}
        >
          Ascending
        </Button>
        <Button
          className="w-100 ms-2"
          variant="red"
          disabled={!isAsc}
          onClick={() => setIsAsc(false)}
        >
          Descending
        </Button>
      </Stack>
    </Container>
  );
}

SortOrderComponent.propTypes = {
  isAsc: PropTypes.bool.isRequired,
  setIsAsc: PropTypes.func.isRequired,
};

export default SortOrderComponent;
