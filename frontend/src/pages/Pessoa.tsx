import { Button, Container, Modal, Row } from 'react-bootstrap';
import Table from 'react-bootstrap/Table';
import './Pessoa.css';
import { useEffect, useState } from 'react';
import webapi from '../services/webapi';
import type Pessoa from '../types/Pessoa';

export default function Pessoa() {
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [editingPessoa, setEditingPessoa] = useState<Pessoa | null>(null);

  const [showModal, setShowModal] = useState(false);
  const handleCloseModal = () => setShowModal(false);
  const handleShowModal = () => setShowModal(true);


  function ShowEditModal(pessoa: Pessoa) {
    setEditingPessoa(pessoa);
    handleShowModal();
  }

  useEffect(() => {
    webapi.get('pessoa')
      .then(response => {
        console.log(response.data);
        setPessoas(response.data);
      })
      .catch(error => {
        console.error('Error fetching data:', error);
      });
  }, []);

  return <Container className='pessoa-container' fluid="md">
    <Row>
      <h1 className="display-4">Pessoas</h1>
    </Row>
    
    <Row>
      <Table striped bordered hover variant="dark" className='pessoa-table'>
        <thead className='table-header'>
          <tr>
            <th>ID</th>
            <th className='tbl-nome'>Nome</th>
            <th>Idade</th>
            <th className='tbl-acoes' colSpan={2}>Ações</th>
          </tr>
        </thead>
        <tbody>
          {pessoas.map(pessoa => (
            <tr key={pessoa.id}>
              <td>{pessoa.id}</td>
              <td>{pessoa.nome}</td>
              <td>{pessoa.idade}</td>
              <td><button className='btn btn-primary' onClick={() => ShowEditModal(pessoa)}>Editar</button></td>
              <td><button className='btn btn-danger'>Excluir</button></td>
            </tr>
          ))}
        </tbody>
      </Table>
    </Row>

    {/* popup */}
    <Modal show={showModal} onHide={handleCloseModal}>
      <Modal.Header closeButton><Modal.Title>{editingPessoa?.nome}</Modal.Title></Modal.Header>
      <Modal.Body>Body content</Modal.Body>
      <Modal.Footer><Button onClick={handleCloseModal}>Close</Button></Modal.Footer>
    </Modal>
  </Container>
}
