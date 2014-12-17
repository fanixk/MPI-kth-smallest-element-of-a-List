MPI-kth-smallest-element-of-a-List
==================================

Algorithm for obtaining the kth smallest element, assuming p processors each with local memory n/p . This is implemented for Distributed memory systems using Message Passing Interface implementation for .net

Implementation of algorithm on Mpi.net and C# based on Paper : 

A Note on Parallel Selection on Coarse-Grained Multicomputers, E. L. G. Saukas and S. W. Song (1999)
==========================================

Consider the selection problem of determining the kth smallest element of a set of n elements. Under the CGM (coarse-grained multicomputer) model with p processors and O(n/p) local memory, we present a deterministic parallel algorithm for the selection problem that requires O(log p) communication rounds. Besides requiring a low number of communication rounds, the algorithm also attempts to minimize the total amount of data transmitted in each round (only O(p) except in the last round). In addition to showing theoretical complexities, we present very promising experimental results obtained on a parallel machine that show almost linear speedup, indicating the efﬁciency and scalability of the proposed algorithm.

