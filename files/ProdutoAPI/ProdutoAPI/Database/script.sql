-- ============================================
-- SCRIPT DE CRIAÇÃO - produto_db
-- MySQL 8.0+
-- ============================================

CREATE DATABASE IF NOT EXISTS produto_db
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE produto_db;

-- ── Tabela de Produtos ────────────────────────────────────
CREATE TABLE IF NOT EXISTS produtos (
    id         INT            NOT NULL AUTO_INCREMENT,
    nome       VARCHAR(100)   NOT NULL,
    preco      DECIMAL(10, 2) NOT NULL,
    quantidade INT            NOT NULL DEFAULT 0,
    PRIMARY KEY (id),
    CONSTRAINT chk_preco      CHECK (preco >= 0.01),
    CONSTRAINT chk_quantidade CHECK (quantidade >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ── Inserts Iniciais ──────────────────────────────────────
INSERT INTO produtos (nome, preco, quantidade) VALUES
    ('Notebook Dell Inspiron 15',  3499.90, 10),
    ('Mouse Logitech MX Master 3',  349.90, 50),
    ('Teclado Mecânico Redragon',   299.90, 35),
    ('Monitor LG 24" Full HD',      899.00, 20),
    ('Headset HyperX Cloud II',     599.90, 15),
    ('Webcam Logitech C920',        399.00,  8),
    ('SSD Kingston 480GB',          229.90, 40),
    ('Memória RAM Corsair 16GB',    289.90, 25);
