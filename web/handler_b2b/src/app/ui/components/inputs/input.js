export default function Input({ input_type, input_name, input_placeholder }) {
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
