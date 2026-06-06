# Bullet Squad - Game Design Document

## Capa

**Nome do jogo:** Bullet Squad  
**Integrante:** Yago Padrao Casatle Giusti Auras  
**Genero:** Acao 2D run-and-gun  
**Engine:** Unity 6.4  
**Plataforma-alvo:** Windows

## Visao Geral

Bullet Squad e um jogo 2D de acao lateral inspirado no ritmo arcade classico de jogos run-and-gun. O jogador controla um soldado de elite enviado para retomar quatro zonas ocupadas por uma forca militar mecanizada.

O foco e oferecer uma experiencia direta, jogavel e fluida: correr, pular, atirar, usar granadas, coletar suprimentos e derrotar um chefe ao final de cada fase.

## Regra Importante da Atividade

O jogo nao usa automacao plena para jogar sozinho e nao depende de player externo para execucao. A interacao principal e humana: o jogador controla movimento, salto, tiros e granadas pelo teclado/mouse dentro de um executavel independente gerado pela Unity.

## Historia

Uma faccao chamada Iron Legion tomou quatro pontos estrategicos usando soldados, drones improvisados e veiculos blindados de sucata militar. O Bullet Squad e enviado para quebrar a linha de suprimentos inimiga antes que a faccao ative uma fabrica automatizada de armas.

Cada fase representa uma zona da ofensiva:

1. **Outpost Road:** entrada da area ocupada, com patrulhas leves.
2. **Rust Bridge:** ponte industrial usada para transporte de armas.
3. **Skyline Ruins:** cidade danificada com plataformas elevadas.
4. **Foundry Core:** fabrica final onde fica o comandante mecanizado.

## Objetivo do Jogador

Avancar pela fase, sobreviver aos inimigos, coletar recursos e derrotar o chefe da missao. Ao vencer um chefe, o jogador avanca para a proxima fase. Ao concluir a quarta fase, o jogo retorna ao menu apos mostrar a vitoria.

## Mecanicas Principais

- Movimento lateral com aceleracao direta.
- Pulo para navegar por plataformas.
- Tiro rapido com municao limitada.
- Granada com dano em area.
- Vida do jogador.
- Pontuacao por inimigos e chefes derrotados.
- Pickups de vida, municao e granadas.
- Inimigos que perseguem e atiram no jogador.
- Chefes com disparos repetidos.

## Controles

- **A/D ou setas:** mover.
- **W, seta para cima ou Space:** pular.
- **J, Ctrl esquerdo ou mouse esquerdo:** atirar.
- **K ou mouse direito:** usar granada.
- **Enter no menu:** iniciar jogo.
- **1-4 no menu:** selecionar fase.

## HUD

O HUD exibe:

- Pontuacao.
- Vida.
- Municao.
- Granadas.
- Missao atual.
- Mensagens de objetivo e conclusao.

## Fases

### Fase 1 - Outpost Road

Introduz movimentacao, tiro, pickups e o primeiro chefe. Dificuldade baixa.

### Fase 2 - Rust Bridge

Mais inimigos e plataformas intermediarias. Incentiva alternar entre tiro e granada.

### Fase 3 - Skyline Ruins

Maior densidade de inimigos e uso de plataformas elevadas.

### Fase 4 - Foundry Core

Ultima missao. Mais inimigos, chefe final e encerramento da campanha.

## Inimigos

- **Enemy Soldier:** patrulheiro que detecta o jogador, se aproxima e dispara.
- **Boss Tank:** chefe de fase com varios pontos de tiro e mais vida.

## Arte e Audio

A primeira versao usa arte autoral simples gerada dentro do projeto com sprites pixelados basicos. Essa abordagem evita dependencia inicial de assets externos e permite evoluir o visual futuramente com sprites mais detalhados.

## Criterios da Atividade

- **Jogabilidade:** movimento, tiro, pulo, granada, pickups, inimigos e chefes.
- **Complexidade:** quatro fases, IA simples, HUD, sistema de vida, pontuacao e transicao de cenas.
- **Documentacao:** este GDD e README do projeto.
- **Fluidez:** controles diretos e cameras seguindo o jogador.
- **Criatividade:** universo proprio com faccao Iron Legion e identidade Bullet Squad.

## Entregaveis Planejados

- Projeto Unity completo no GitHub publico.
- Executavel Windows independente.
- GDD com nome do integrante.
- Snapshots do jogo em execucao e texto explicativo/storyboard.
