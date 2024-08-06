import PropTypes from 'prop-types';

function Input({ input_type, input_name, input_placeholder }) {
  return (
    <input
      className="bg-gray-primary hover:placeholder:text-gray-super_dark text-black rounded-lg shadow-md placeholder:text-gray-secondary p-2.5 min-w-[259px]"
      type={input_type}
      name={input_name}
      placeholder={input_placeholder}
      required
    />
  );
}

Input.propTypes = {
  input_type: PropTypes.string.isRequired,
  input_name: PropTypes.string.isRequired,
  input_placeholder: PropTypes.string
}

export default Input