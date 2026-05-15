function displayInDiv(value) {
  const div = document.getElementById('output');
  if (!div) {
    throw new Error('Div element with id "output" not found.');
  }
  div.textContent = value;
}
