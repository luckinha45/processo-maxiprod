import { BrowserRouter, Route, Routes } from 'react-router';
import Header from './components/Header';
import Home from './pages/Home';
import Pessoa from './pages/Pessoa';
import Categoria from './pages/Categoria';
import Transacoes from './pages/Transacoes';

function App() {
  return (
    <>
      <BrowserRouter>
        <Header />
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/pessoa" element={<Pessoa />} />
          <Route path="/categoria" element={<Categoria />} />
          <Route path="/transacoes" element={<Transacoes />} />
        </Routes>
      </BrowserRouter>
    </>
  )
}

export default App
