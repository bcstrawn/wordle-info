import React, { Component } from 'react';

export class FetchData extends Component {
  static displayName = FetchData.name;

  constructor(props) {
    super(props);
    this.state = { guesses: [], loading: true, sort: "Alpha", second: false, search: "", firstWord: "" };
    this.importGuesses = this.importGuesses.bind(this);
    this.importSecondGuesses = this.importSecondGuesses.bind(this);
    this.importVocab = this.importVocab.bind(this);
    this.getCount = this.getCount.bind(this);
    this.processGuesses = this.processGuesses.bind(this);
    this.processSecondGuesses = this.processSecondGuesses.bind(this);
    this.toggleSecondGuess = this.toggleSecondGuess.bind(this);
    this.getLetters = this.getLetters.bind(this);

    this.handleChange = this.handleChange.bind(this);
    this.handleChange2 = this.handleChange2.bind(this);
  }

  componentDidMount() {
    this.populateWeatherData();
  }

  renderGuesses(guesses) {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th onClick={() => this.setSort("Alpha")}>Word</th>
            <th onClick={() => this.setSort("Green")}>Green</th>
            <th onClick={() => this.setSort("Yellow")}>Yellow</th>
            <th onClick={() => this.setSort("Total")}>Total</th>
          </tr>
        </thead>
        <tbody>
          {guesses.map(guess =>
            <tr key={guess.id}>
              <td>{guess.value}</td>
              <td>{guess.averageGreen}</td>
              <td>{guess.averageYellow}</td>
              <td>{guess.total}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : this.renderGuesses(this.state.guesses);

    return (
      <div>
        <h1 id="tabelLabel" >Wordle</h1>
        
        <button className="btn btn-primary" onClick={this.importVocab} style={{ "marginRight": "5px" }}>Import Vocab</button>
        <button className="btn btn-primary" onClick={this.importGuesses} style={{ "marginRight": "5px" }}>Import Guesses</button>
        <button className="btn btn-primary" onClick={this.importSecondGuesses} style={{ "marginRight": "5px" }}>Import Second Guesses</button>
        <button className="btn btn-primary" onClick={this.getCount} style={{ "marginRight": "5px" }}>Count</button>
        <button className="btn btn-primary" onClick={this.processGuesses} style={{ "marginRight": "5px" }}>Process</button>
        <input type="text" value={this.state.firstWord} onChange={this.handleChange2} placeholder='First Word' />
        <button className="btn btn-primary" onClick={this.processSecondGuesses} style={{ "marginRight": "5px" }}>Process 2</button>
        <button className="btn btn-primary" onClick={this.toggleSecondGuess} style={{ "marginRight": "5px" }}>{this.state.second ? 'Regular' : 'Second'}</button>

        <input type="text" value={this.state.search} onChange={this.handleChange} placeholder='Search' />
        <button className="btn btn-primary" onClick={this.getLetters} style={{ "marginRight": "5px" }}>Letters</button>

        {contents}
      </div>
    );
  }

  setSort(newSort)
  {
    this.setState({ sort: newSort });
    this.populateWeatherData(newSort);
  }

  handleChange(event)
  {
    this.setState({ search: event.target.value });
  }

  handleChange2(event)
  {
    this.setState({ firstWord: event.target.value });
  }

  async populateWeatherData(manualSort) {
    var sort = manualSort || this.state.sort;
    var url = this.state.second ? `weatherforecast/GetSecondGuesses?sort=${sort}` : `weatherforecast/GetGuesses?sort=${sort}`;
    if (this.state.search)
      url = url + `&search=${this.state.search}`;
    const response = await fetch(url);
    const data = await response.json();
    this.setState({ guesses: data, loading: false });
  }

  async getCount() {
    const response = await fetch('weatherforecast/Count');
    const data = await response.json();
    console.log(data);
  }

  async getLetters() {
    const response = await fetch('weatherforecast/GetLetters');
    const data = await response.json();
    this.setState({ guesses: data, loading: false });
  }

  async importGuesses() {
    console.log("Hey do the import");
    const response = await fetch('weatherforecast/ImportGuesses', { method: 'POST' });
  }

  async importSecondGuesses() {
    console.log("Hey do the import");
    const response = await fetch('weatherforecast/ImportSecondGuesses', { method: 'POST' });
  }

  async importVocab() {
    console.log("Hey do the other import");
    const response = await fetch('weatherforecast/ImportVocab', { method: 'POST' });
  }

  async processGuesses() {
    console.log("Hey do the process");
    const response = await fetch('weatherforecast/ProcessGuesses', { method: 'POST' });
  }

  async processSecondGuesses() {
    console.log("Hey do the process");
    // const response = await fetch('weatherforecast/ProcessSecondGuesses', { method: 'POST', body: JSON.stringify({ firstWord: this.state.firstWord }), headers: {
    //   'Content-Type': 'application/json'
    // } });
    const response = await fetch('weatherforecast/ProcessSecondGuesses', { method: 'POST', body: `"${this.state.firstWord}"`, headers: {
      'Content-Type': 'application/json'
    } });
  }

  toggleSecondGuess()
  {
    if (this.state.second)
      this.setState({ second: false });
    else
      this.setState({ second: true });
  }
}
