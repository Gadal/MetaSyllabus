// DATA
// --------------------------------

// Petersen graph vertices
//           0          
//                    
//           1          
//                    
// 2    3         4    5
//                     
//                     
//        6     7       
//                    
//     8           9    

// See also: https://en.wikipedia.org/wiki/Automorphism
var PETERSEN_GRAPH_AUTOMORPHISM_GROUP = 
  [
    [0, 1, 2, 3, 4, 5, 6, 7, 8, 9],
    [0, 1, 2, 8, 9, 5, 7, 6, 3, 4],
    [0, 1, 5, 4, 3, 2, 7, 6, 9, 8],
    [0, 1, 5, 9, 8, 2, 6, 7, 4, 3],
    [0, 2, 1, 6, 4, 5, 3, 8, 7, 9],
    [0, 2, 1, 7, 9, 5, 8, 3, 6, 4],
    [0, 2, 5, 4, 6, 1, 8, 3, 9, 7],
    [0, 2, 5, 9, 7, 1, 3, 8, 4, 6],
    [0, 5, 1, 6, 8, 2, 9, 4, 7, 3],
    [0, 5, 1, 7, 3, 2, 4, 9, 6, 8],
    [0, 5, 2, 3, 7, 1, 9, 4, 8, 6],
    [0, 5, 2, 8, 6, 1, 4, 9, 3, 7],
    [1, 0, 6, 4, 3, 7, 2, 5, 8, 9],
    [1, 0, 6, 8, 9, 7, 5, 2, 4, 3],
    [1, 0, 7, 3, 4, 6, 5, 2, 9, 8],
    [1, 0, 7, 9, 8, 6, 2, 5, 3, 4],
    [1, 6, 0, 2, 3, 7, 4, 8, 5, 9],
    [1, 6, 0, 5, 9, 7, 8, 4, 2, 3],
    [1, 6, 7, 3, 2, 0, 8, 4, 9, 5],
    [1, 6, 7, 9, 5, 0, 4, 8, 3, 2],
    [1, 7, 0, 2, 8, 6, 9, 3, 5, 4],
    [1, 7, 0, 5, 4, 6, 3, 9, 2, 8],
    [1, 7, 6, 4, 5, 0, 9, 3, 8, 2],
    [1, 7, 6, 8, 2, 0, 3, 9, 4, 5],
    [2, 0, 3, 4, 6, 8, 1, 5, 7, 9],
    [2, 0, 3, 7, 9, 8, 5, 1, 4, 6],
    [2, 0, 8, 6, 4, 3, 5, 1, 9, 7],
    [2, 0, 8, 9, 7, 3, 1, 5, 6, 4],
    [2, 3, 0, 1, 6, 8, 4, 7, 5, 9],
    [2, 3, 0, 5, 9, 8, 7, 4, 1, 6],
    [2, 3, 8, 6, 1, 0, 7, 4, 9, 5],
    [2, 3, 8, 9, 5, 0, 4, 7, 6, 1],
    [2, 8, 0, 1, 7, 3, 9, 6, 5, 4],
    [2, 8, 0, 5, 4, 3, 6, 9, 1, 7],
    [2, 8, 3, 4, 5, 0, 9, 6, 7, 1],
    [2, 8, 3, 7, 1, 0, 6, 9, 4, 5],
    [3, 2, 4, 5, 9, 7, 8, 0, 6, 1],
    [3, 2, 4, 6, 1, 7, 0, 8, 5, 9],
    [3, 2, 7, 1, 6, 4, 8, 0, 9, 5],
    [3, 2, 7, 9, 5, 4, 0, 8, 1, 6],
    [3, 4, 2, 0, 1, 7, 6, 5, 8, 9],
    [3, 4, 2, 8, 9, 7, 5, 6, 0, 1],
    [3, 4, 7, 1, 0, 2, 5, 6, 9, 8],
    [3, 4, 7, 9, 8, 2, 6, 5, 1, 0],
    [3, 7, 2, 0, 5, 4, 9, 1, 8, 6],
    [3, 7, 2, 8, 6, 4, 1, 9, 0, 5],
    [3, 7, 4, 5, 0, 2, 1, 9, 6, 8],
    [3, 7, 4, 6, 8, 2, 9, 1, 5, 0],
    [4, 3, 5, 0, 1, 6, 7, 2, 9, 8],
    [4, 3, 5, 9, 8, 6, 2, 7, 0, 1],
    [4, 3, 6, 1, 0, 5, 2, 7, 8, 9],
    [4, 3, 6, 8, 9, 5, 7, 2, 1, 0],
    [4, 5, 3, 2, 8, 6, 9, 0, 7, 1],
    [4, 5, 3, 7, 1, 6, 0, 9, 2, 8],
    [4, 5, 6, 1, 7, 3, 9, 0, 8, 2],
    [4, 5, 6, 8, 2, 3, 0, 9, 1, 7],
    [4, 6, 3, 2, 0, 5, 1, 8, 7, 9],
    [4, 6, 3, 7, 9, 5, 8, 1, 2, 0],
    [4, 6, 5, 0, 2, 3, 8, 1, 9, 7],
    [4, 6, 5, 9, 7, 3, 1, 8, 0, 2],
    [5, 0, 4, 3, 7, 9, 1, 2, 6, 8],
    [5, 0, 4, 6, 8, 9, 2, 1, 3, 7],
    [5, 0, 9, 7, 3, 4, 2, 1, 8, 6],
    [5, 0, 9, 8, 6, 4, 1, 2, 7, 3],
    [5, 4, 0, 1, 7, 9, 3, 6, 2, 8],
    [5, 4, 0, 2, 8, 9, 6, 3, 1, 7],
    [5, 4, 9, 7, 1, 0, 6, 3, 8, 2],
    [5, 4, 9, 8, 2, 0, 3, 6, 7, 1],
    [5, 9, 0, 1, 6, 4, 8, 7, 2, 3],
    [5, 9, 0, 2, 3, 4, 7, 8, 1, 6],
    [5, 9, 4, 3, 2, 0, 8, 7, 6, 1],
    [5, 9, 4, 6, 1, 0, 7, 8, 3, 2],
    [6, 1, 4, 3, 2, 8, 0, 7, 5, 9],
    [6, 1, 4, 5, 9, 8, 7, 0, 3, 2],
    [6, 1, 8, 2, 3, 4, 7, 0, 9, 5],
    [6, 1, 8, 9, 5, 4, 0, 7, 2, 3],
    [6, 4, 1, 0, 2, 8, 3, 5, 7, 9],
    [6, 4, 1, 7, 9, 8, 5, 3, 0, 2],
    [6, 4, 8, 2, 0, 1, 5, 3, 9, 7],
    [6, 4, 8, 9, 7, 1, 3, 5, 2, 0],
    [6, 8, 1, 0, 5, 4, 9, 2, 7, 3],
    [6, 8, 1, 7, 3, 4, 2, 9, 0, 5],
    [6, 8, 4, 3, 7, 1, 9, 2, 5, 0],
    [6, 8, 4, 5, 0, 1, 2, 9, 3, 7],
    [7, 1, 3, 2, 8, 9, 6, 0, 4, 5],
    [7, 1, 3, 4, 5, 9, 0, 6, 2, 8],
    [7, 1, 9, 5, 4, 3, 6, 0, 8, 2],
    [7, 1, 9, 8, 2, 3, 0, 6, 5, 4],
    [7, 3, 1, 0, 5, 9, 4, 2, 6, 8],
    [7, 3, 1, 6, 8, 9, 2, 4, 0, 5],
    [7, 3, 9, 5, 0, 1, 2, 4, 8, 6],
    [7, 3, 9, 8, 6, 1, 4, 2, 5, 0],
    [7, 9, 1, 0, 2, 3, 8, 5, 6, 4],
    [7, 9, 1, 6, 4, 3, 5, 8, 0, 2],
    [7, 9, 3, 2, 0, 1, 5, 8, 4, 6],
    [7, 9, 3, 4, 6, 1, 8, 5, 2, 0],
    [8, 2, 6, 1, 7, 9, 3, 0, 4, 5],
    [8, 2, 6, 4, 5, 9, 0, 3, 1, 7],
    [8, 2, 9, 5, 4, 6, 3, 0, 7, 1],
    [8, 2, 9, 7, 1, 6, 0, 3, 5, 4],
    [8, 6, 2, 0, 5, 9, 4, 1, 3, 7],
    [8, 6, 2, 3, 7, 9, 1, 4, 0, 5],
    [8, 6, 9, 5, 0, 2, 1, 4, 7, 3],
    [8, 6, 9, 7, 3, 2, 4, 1, 5, 0],
    [8, 9, 2, 0, 1, 6, 7, 5, 3, 4],
    [8, 9, 2, 3, 4, 6, 5, 7, 0, 1],
    [8, 9, 6, 1, 0, 2, 5, 7, 4, 3],
    [8, 9, 6, 4, 3, 2, 7, 5, 1, 0],
    [9, 5, 7, 1, 6, 8, 4, 0, 3, 2],
    [9, 5, 7, 3, 2, 8, 0, 4, 1, 6],
    [9, 5, 8, 2, 3, 7, 4, 0, 6, 1],
    [9, 5, 8, 6, 1, 7, 0, 4, 2, 3],
    [9, 7, 5, 0, 2, 8, 3, 1, 4, 6],
    [9, 7, 5, 4, 6, 8, 1, 3, 0, 2],
    [9, 7, 8, 2, 0, 5, 1, 3, 6, 4],
    [9, 7, 8, 6, 4, 5, 3, 1, 2, 0],
    [9, 8, 5, 0, 1, 7, 6, 2, 4, 3],
    [9, 8, 5, 4, 3, 7, 2, 6, 0, 1],
    [9, 8, 7, 1, 0, 5, 2, 6, 3, 4],
    [9, 8, 7, 3, 4, 5, 6, 2, 1, 0]
  ];

// Draw a line between each of these pairs, and you end up with the Petersen Graph
// That is, EDGE_SET describes a hamiltonian walk of the Petersen Graph.
var EDGE_SET =
  [
    [0, 1],
    [0, 2],
    [0, 5],
    [1, 6],
    [1, 7],
    [2, 3],
    [2, 8],
    [3, 4],
    [3, 7],
    [4, 5],
    [4, 6],
    [5, 9],
    [6, 8],
    [7, 9],
    [8, 9]
  ];

// x, y coordinates.
// Describes a set of aesthetically pleasing configurations.
var CONFIGURATIONS =
    [
      [
        [0.51, 0.00],
        [0.51, 0.27],
        [0.00, 0.37],
        [0.25, 0.47],
        [0.75, 0.47],
        [1.00, 0.37],
        [0.35, 0.77],
        [0.65, 0.77],
        [0.20, 1.00],
        [0.81, 1.00]
      ],
      [
        [0.89, 0.00],
        [0.19, 0.00],
        [0.67, 0.19],
        [0.00, 0.19],
        [0.34, 0.51],
        [1.00, 0.51],
        [0.00, 0.81],
        [0.19, 1.00],
        [0.67, 0.81],
        [0.89, 1.00]
      ],
      [
        [0.50, 0.50],
        [0.50, 0.21],
        [0.29, 0.63],
        [0.75, 1.00],
        [0.25, 1.00],
        [0.72, 0.63],
        [0.00, 0.50],
        [1.00, 0.50],
        [0.25, 0.00],
        [0.75, 0.00]
      ],
      [
        [0.50, 0.00],
        [0.50, 1.00],
        [0.85, 0.15],
        [0.25, 0.35],
        [0.75, 0.35],
        [0.15, 0.15],
        [0.85, 0.85],
        [0.15, 0.85],
        [1.00, 0.50],
        [0.00, 0.50]
      ],
      [
        [0.50, 0.52],
        [0.50, 0.00],
        [0.06, 0.77],
        [0.32, 1.00],
        [0.67, 1.00],
        [0.94, 0.77],
        [0.17, 0.12],
        [0.82, 0.12],
        [0.00, 0.42],
        [1.00, 0.42]
      ],
      [
        [0.50, 0.55],
        [0.17, 0.33],
        [0.82, 0.33],
        [1.00, 0.67],
        [0.82, 1.00],
        [0.50, 1.00],
        [0.34, 0.00],
        [0.00, 0.67],
        [0.65, 0.00],
        [0.17, 1.00]
      ],
      [
        [0.50, 0.00],
        [0.50, 1.00],
        [0.00, 0.00],
        [0.00, 0.50],
        [1.00, 0.50],
        [1.00, 0.00],
        [1.00, 1.00],
        [0.00, 1.00],
        [0.25, 0.25],
        [0.75, 0.25]
      ]
    ];

// GLOBALS
// --------------------------------

var CANVAS = document.getElementById('logo');
var CONTEXT = CANVAS.getContext('2d');

var LINE_THICKNESS = 4;
var LINE_COLOUR = 'white';

CONTEXT.shadowColor = "rgba( 0, 0, 0, 0.3 )";
CONTEXT.shadowOffsetX = LINE_THICKNESS * 3/5;
CONTEXT.shadowOffsetY = LINE_THICKNESS * 3/5;
CONTEXT.shadowBlur    = LINE_THICKNESS * 2/3;

var SCALING_FACTOR = Math.min(CANVAS.width, CANVAS.height) - 2 * LINE_THICKNESS;

var LOOP_DURATION_MS = 3500;

var START_GRAPH = getGraphPermutation(0, 0);
var END_GRAPH   = getRandomGraphPermutation();

var START_TIME = null;

CONTEXT.lineCap = 'round';
CONTEXT.lineWidth = LINE_THICKNESS;
CONTEXT.strokeStyle = LINE_COLOUR;

// MAIN
// --------------------------------

// Draw the first frame so there's something on the screen while we wait
drawLines(0);

// Wait for a few seconds, then begin the animation
setTimeout(function() { requestAnimationFrame(animate); }, LOOP_DURATION_MS/2)

// HELPERS
// --------------------------------

// Draws one frame of the animation
function animate(timestamp) {
  // requestAnimationFrame() passes the current time, "timestamp", to animate() when it calls animate().
   
  // Determine how far through the animation you are, between 0 and 1 (0% and 100%)
  if (START_TIME === null) START_TIME = timestamp;
  var elapsedTime = timestamp - START_TIME;
  var progress = Math.min(elapsedTime/LOOP_DURATION_MS, 1);
  progress = easeOutQuad(progress);
  
  if (elapsedTime < LOOP_DURATION_MS) {

    // Clear previous frame
    CONTEXT.clearRect(0, 0, CANVAS.width, CANVAS.height);
    
    // Draw the new frame
    drawLines(progress);
    
  } else { 
    // Select a new target to animate to, then restart the animation cycle
    START_TIME = null;
    START_GRAPH = END_GRAPH;
    END_GRAPH = getRandomGraphPermutation();
  }
  
  requestAnimationFrame(animate);
}

function drawLines(progress) {
  CONTEXT.beginPath();
  
  for(i = 0; i < EDGE_SET.length; i++)
  {
    // Draw each line somewhere between where it started, and where we want it to end up.
    var line = {
      x1: (1 - progress) * START_GRAPH[i].x1 + progress * END_GRAPH[i].x1,
      y1: (1 - progress) * START_GRAPH[i].y1 + progress * END_GRAPH[i].y1,
      x2: (1 - progress) * START_GRAPH[i].x2 + progress * END_GRAPH[i].x2,
      y2: (1 - progress) * START_GRAPH[i].y2 + progress * END_GRAPH[i].y2
    };
    
    CONTEXT.moveTo(line.x1, line.y1);
    CONTEXT.lineTo(line.x2, line.y2);
  }
  
  CONTEXT.stroke();
}

// Calculates the edge mapping for a given automorphism and configuration
function getLine(edgeIndex, automorphismIndex, configIndex) {
  /* Let's break this down by parts:
     
     EDGE_SET[A][B]
     A: The petersen graph has fifteen edges.  This simply selects one of the edges.
     B: Each edge has a start point and end point. If B is 0, we get the start point.  If B is 1, we get the end.
     
     PETERSEN_GRAPH_AUTOMORPHISM_GROUP[A][B]
     A: The automorphism index is an int from 0 to 119.  The automorphism index determines how to rearrange
        the vertices of the graph so that it's still the petersen graph when you're done.
        Google "automorphism group" for more information.
     B: EDGE_SET[edgeIndex][#] is the number of a vertex in the graph.  Treating it as an index in an array,
        we're performing a vertex mapping from one automorphism to another.
        
     CONFIGURATIONS[A][B][C]
     A: CONFIGURATIONS contains various x,y coordinates for drawing the petersen graph in various ways.
        So, for example, CONFIGURATIONS[0] draws the graph as a pentagon; CONFIGURATIONS[3] draws it as a square.
     B: This selects which x,y coordinate we're interested in.
     C: This selects whether we want x or y.
     
     SCALING_FACTOR and LINE_THICKNESS are modifiers used to make the animation fit neatly inside the canvas.
   */
  
  configuration =                    CONFIGURATIONS[configIndex]
  automorphism  = PETERSEN_GRAPH_AUTOMORPHISM_GROUP[automorphismIndex]
  edge          =                          EDGE_SET[edgeIndex]
  
  var line = {
    x1: configuration[automorphism[edge[0]]][0] * SCALING_FACTOR + LINE_THICKNESS/2,
    y1: configuration[automorphism[edge[0]]][1] * SCALING_FACTOR + LINE_THICKNESS/2,
    x2: configuration[automorphism[edge[1]]][0] * SCALING_FACTOR + LINE_THICKNESS/2,
    y2: configuration[automorphism[edge[1]]][1] * SCALING_FACTOR + LINE_THICKNESS/2
  };
  
  return line;
}

function getRandomGraphPermutation() {
  return getGraphPermutation(Math.floor(Math.random() * PETERSEN_GRAPH_AUTOMORPHISM_GROUP.length), 
                             Math.floor(Math.random() * CONFIGURATIONS.length));
}

function getGraphPermutation(automorphism, configuration) {
  var permutation = [];
  
  for(i = 0; i < EDGE_SET.length; i++)
  {
    var newLine = getLine(i, automorphism, configuration);
    permutation.push(newLine);
  }
  
  return permutation;
}

function easeOutQuad(t) { 
  t = Math.max(0, t);
  t = Math.min(t, 1);
  return t*(2-t)
}
