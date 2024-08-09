import { Container, Form, Button, Stack, InputGroup } from "react-bootstrap";
import Image from "next/image";
import search_icons from "../../../public/icons/icon_search.png";

export default function SearchBar() {
  const barStyle = {
    "border-radius": "60px",
    "max-width": "701px",
  };
  return (
    <Container className="sec-blue-bg" style={barStyle}>
      <Form>
        <Stack direction="horizontal">
          <InputGroup>
            <Form.Control
              className="input-style-search"
              placeholder="Search"
              aria-label="Search"
            />
          </InputGroup>
          <Button variant="as-link" type="submit" className="ms-auto pe-0 py-0">
            <Image src={search_icons} alt="search icon" />
          </Button>
        </Stack>
      </Form>
    </Container>
  );
}
