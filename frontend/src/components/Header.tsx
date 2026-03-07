import Nav from 'react-bootstrap/esm/Nav';
import Navbar from 'react-bootstrap/esm/Navbar';
import { Link } from "react-router-dom";
import './Header.css';

export default function Header() {
  return (
    <Navbar className="header" bg="" data-bs-theme="dark">
      <Navbar.Brand>Gerencia.me</Navbar.Brand>
      <Nav className="me-auto">
        <Nav.Link className="px-3" as={Link} to="/">Inicio</Nav.Link>
        <Nav.Link className="px-3" as={Link} to="/pessoa">Pessoas</Nav.Link>
        <Nav.Link className="px-3" as={Link} to="/categoria">Categorias</Nav.Link>
        <Nav.Link className="px-3" as={Link} to="/transacoes">Transações</Nav.Link>
        <Nav.Link className="px-3" as={Link} to="/rel-pessoas">Rel. Pessoas</Nav.Link>
        <Nav.Link className="px-3" as={Link} to="/rel-categorias">Rel. Categorias</Nav.Link>
      </Nav>
    </Navbar>
  )
}
