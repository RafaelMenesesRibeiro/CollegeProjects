;;##############################################################################
;;==============================================================================
;; GRUPO 1
;; 78375 - Joao Pirralha
;; 84758 - Rafael Ribeiro
;;==============================================================================
;;##############################################################################

(in-package :user)

;; Note: in our internal representation the order of the tasks in a shift is reversed!
;; (Due to cons.) It's reversed again for the external representation.
(defstruct afectacao-state problem allocated non-allocated cost total-cost heuristic heuristic-function hash)
(defstruct search-node state predecessor f)

(defconstant travel-time 40)
(defconstant lunch-time 40)
(defconstant 4-hours 240)
(defconstant 6-hours 360)
(defconstant 8-hours 480)
(defconstant L1 1)

(defconstant greedy-allocation-heuristic-max-duration 420) ;; 7 hours, if less it loses some optimality
(defconstant it-samp-max-time 290)
(defconstant ilds-max-time 240)


;;##############################################################################
;;==============================================================================
;; MODELLATION
;;==============================================================================
;;##############################################################################

;;==============================================================================
;; AUXILIAR RESTRICTION VERIFICATION MODELLATION FUNCTIONS
;;==============================================================================

;; When it can be checked before forming the new shift
(defun shift-task-ok-p (state shift taskn)
    ;; If it's a new shift it's always OK
    (if (not shift) (return-from shift-task-ok-p T))
    
    ;; Restriction "0" - spatial continuity:
    (let ((transport-time travel-time))
        ;; (first shift) is the previous task, as the shifts are reversed in our internal representation
        (if (= (get-task-end-local state (first shift)) (get-task-start-local state taskn))
            (setf transport-time 0))
        (if (< (get-task-start-time state taskn) (+ (get-task-end-time state (first shift)) transport-time))
            (return-from shift-task-ok-p NIL)))
    
    T)


;; When it's easier to check when the new shift is formed
(defun new-shift-ok-p (state new-shift)
    ;; Restriction 2 - max shift duration (8 hours):
    (if (> (get-shift-duration state new-shift) 8-hours)
        (return-from new-shift-ok-p NIL))
    
    ;; Restrictions 4/5/6/7
    (return-from new-shift-ok-p (lunch-break-ok-p state new-shift)))


(defun lunch-break-ok-p (state shift)
    (if (< (get-shift-duration-lite state shift) 4-hours)
        (return-from lunch-break-ok-p T))
    
    (let ((has-break NIL) (previous-start-local) (previous-start-time) (block-end-time))
        (dolist (taskn shift)
            (if previous-start-local
                (let ((difference (- previous-start-time (get-task-end-time state taskn))))
                    
                    ;; Restriction 6 - worker can't have lunch while transported:
                    (if (/= previous-start-local (get-task-end-local state taskn))
                        (setf difference (- difference travel-time)))
                    
                    ;; Restriction 5 - lunch break should have 40 minutes:
                    (if (>= difference lunch-time)
                        (progn
                            ;; Restriction 4 - only one pause for lunch
                            (if has-break (return-from lunch-break-ok-p NIL))
                            
                            (setf has-break T)
                            ;; Can't return immediatly, still has to check the
                            ;; remaining blocks, to see if they don't exceed 4 hours
                            (setf block-end-time (get-task-end-time state taskn)))
                        (progn
                            ;; Restriction 7 - maximum driving time before lunch break is 4 hours:
                            (if (> (- block-end-time (get-task-start-time state taskn)) 4-hours) (progn
                            (return-from lunch-break-ok-p NIL))))))
                
                ;; First task
                (setf block-end-time (get-task-end-time state taskn)))
            (setf previous-start-local (get-task-start-local state taskn))
            (setf previous-start-time (get-task-start-time state taskn)))
        has-break))


;;==============================================================================
;; OTHER AUXILIAR MODELLATION FUNCTIONS
;;==============================================================================

(defun get-task-start-local (state taskn) (aref (afectacao-state-problem state) taskn 0))
(defun get-task-end-local (state taskn) (aref (afectacao-state-problem state) taskn 1))
(defun get-task-start-time (state taskn) (aref (afectacao-state-problem state) taskn 2))
(defun get-task-end-time (state taskn) (aref (afectacao-state-problem state) taskn 3))
(defun get-task-duration (state taskn) (- (aref (afectacao-state-problem state) taskn 3) (aref (afectacao-state-problem state) taskn 2)))


(defun get-shift-duration (state shift)
    (if (not shift) 0
        ;; start-taskn and end-taskn are guaranteed by restriction 0
        (let ((duration) (start-taskn (first (last shift))) (end-taskn (first shift)))
            (setf duration (- (get-task-end-time state end-taskn) (get-task-start-time state start-taskn)))
            ;; Restriction 1 (via 8b) -- shift must start and end at L1, if not add 40 minutes (for each):
            ;; End confirmed with professor
            (if (/= (get-task-start-local state start-taskn) L1) (incf duration travel-time))
            (if (/= (get-task-end-local state end-taskn) L1) (incf duration travel-time))
            (max 6-hours duration))))


(defun get-shift-duration-lite (state shift)
    (if (not shift) 0
        ;; start-taskn and end-taskn are guaranteed by restriction 0
        (let ((duration) (start-taskn (first (last shift))) (end-taskn (first shift)))
            (setf duration (- (get-task-end-time state end-taskn) (get-task-start-time state start-taskn)))
            ;; Restriction 1 (via 8b):
            (if (/= (get-task-start-local state start-taskn) L1) (incf duration travel-time))
            (if (/= (get-task-end-local state end-taskn) L1) (incf duration travel-time))
            duration)))


(defun get-new-shift-transition-cost (state old-shift new-shift)
    (- (get-shift-duration state new-shift) (get-shift-duration state old-shift)))


(defun create-successor (state shift taskn new-shift)
    (let ((new-state (make-afectacao-state :problem (afectacao-state-problem state)
            :cost (get-new-shift-transition-cost state shift new-shift)
            :heuristic-function (afectacao-state-heuristic-function state)))
        (shifts-rest (remove shift (afectacao-state-allocated state) :test #'eq :count 1)))
        
        (setf (afectacao-state-total-cost new-state) (+ (afectacao-state-total-cost state) (afectacao-state-cost new-state)))
        (setf (afectacao-state-allocated new-state) (cons new-shift shifts-rest))
        (setf (afectacao-state-non-allocated new-state) (remove taskn (afectacao-state-non-allocated state) :test #'= :count 1))
        (setf (afectacao-state-hash new-state) (logxor (logxor (afectacao-state-hash state) (sxhash shift)) (sxhash new-shift)))
        (setf (afectacao-state-heuristic new-state) (funcall (afectacao-state-heuristic-function new-state) new-state))
        new-state))


;;==============================================================================
;; MAIN MODELLATION FUNCTIONS
;;==============================================================================

(defun successors (state)
    (let ((successors-list))
        (dolist (shift (cons NIL (afectacao-state-allocated state)))
            (dolist (taskn (afectacao-state-non-allocated state))
                (if (shift-task-ok-p state shift taskn)
                    (let ((new-shift (cons taskn shift)))
                        (if (new-shift-ok-p state new-shift)
                            (setf successors-list (cons (create-successor state shift taskn new-shift) successors-list)))))))
        successors-list))


;; Only returns the successors within best+(worst-best)*fraction,
;; plus some noise due to the iterative approach to avoid sorting all in memory
(defun successors-best (state &key (fraction (/ 1.0 16.0)))
    (let ((successors-list) (best MOST-POSITIVE-FIXNUM) (worst MOST-NEGATIVE-FIXNUM))
        (dolist (shift (cons NIL (afectacao-state-allocated state)))
            (dolist (taskn (afectacao-state-non-allocated state))
                (if (shift-task-ok-p state shift taskn)
                    (let ((new-shift (cons taskn shift)))
                        (if (new-shift-ok-p state new-shift)
                            (let ((new-successor (create-successor state shift taskn new-shift)) (new-successor-cost))
                                (setf new-successor-cost (+ (afectacao-state-total-cost new-successor) (afectacao-state-heuristic new-successor)))
                                (if (<= new-successor-cost (+ best (* (- worst best) fraction)))
                                    (setf successors-list (cons new-successor successors-list)))
                                (setf best (min best new-successor-cost))
                                (setf worst (max worst new-successor-cost))))))))
        successors-list))


(defun objective? (state) (not (afectacao-state-non-allocated state)))


(defun state= (a b) (= (afectacao-state-hash a) (afectacao-state-hash b)))


;;##############################################################################
;;==============================================================================
;; HEURISTICS
;;==============================================================================
;;##############################################################################

(defun fast-heuristic (state &key (multiplier 5))
    (let ((duration-sum 0) (previous-taskn))
        (dolist (taskn (afectacao-state-non-allocated state))
            (incf duration-sum (get-task-duration state taskn))
            ;; Penalize overlaps
            (if (and previous-taskn (<= (get-task-start-time state taskn) (get-task-end-time state previous-taskn))
                (>= (get-task-end-time state taskn) (get-task-start-time state previous-taskn)))
                (incf duration-sum (- (get-task-end-time state previous-taskn) (get-task-start-time state taskn))))
            (setf previous-taskn taskn))
        (* multiplier duration-sum)))


(defun greedy-allocation-heuristic (state)
    (let ((shifts (copy-list (afectacao-state-allocated state)))
        (non-allocated (afectacao-state-non-allocated state))
        (accumulator 0))
        
        (dolist (taskn non-allocated)
            (let ((best-shift) (best-shift-new) (best-duration MOST-POSITIVE-FIXNUM))
                (dolist (shift shifts)
                    (if (shift-task-ok-p state shift taskn)
                        (let ((new-shift (cons taskn shift)))
                            (if (new-shift-ok-p state new-shift)
                                (let ((duration (get-shift-duration-lite state new-shift)))
                                    (if (< duration best-duration)
                                        (progn
                                            (setf best-shift shift)
                                            (setf best-shift-new new-shift)
                                            (setf best-duration duration))))))))
                
                (if best-shift
                    (progn
                        (setf shifts (delete best-shift shifts :test #'eq :count 1))
                        (if (>= best-duration greedy-allocation-heuristic-max-duration)
                            (incf accumulator best-duration)
                            (setf shifts (cons best-shift-new shifts))))
                    (setf shifts (cons (list taskn) shifts)))))
        
        (dolist (shift shifts)
            (incf accumulator (get-shift-duration state shift)))
        ;; The total cost is already included in the estimate
        (- accumulator (afectacao-state-total-cost state))))


;;##############################################################################
;;==============================================================================
;; CUSTOM SEARCHES
;;==============================================================================
;;##############################################################################

(defun get-solution-list (solution)
    (let ((solution-list) (current-node solution))
        (loop while current-node do
            (setf solution-list (cons (search-node-state current-node) solution-list))
            (setf current-node (search-node-predecessor current-node)))
        solution-list))


;;==============================================================================
;; Iterative Sampling
;;==============================================================================

(defvar *is-expansions*)
(defvar *is-generations*)


(defun is-result-better (solution new-solution)
    (if (eql new-solution NIL) (return-from is-result-better NIL))
    (if (eql solution NIL) (return-from is-result-better T))
    (let ((cost (afectacao-state-total-cost solution)) (new-cost (afectacao-state-total-cost new-solution)))
        (if (< new-cost cost) (return-from is-result-better T))))


(defun it-samp-probe (state)
    (incf *is-expansions*)
    (if (objective? state) (return-from it-samp-probe state))
    (let ((successors (successors state)) (random-child NIL))
        (incf *is-generations* (length successors))
        (if (eql successors NIL) (return-from it-samp-probe state))
        (setf random-child (nth (random (length successors)) successors))
        (setf successors NIL)
        (return-from it-samp-probe (it-samp-probe random-child))))


(defun it-samp (initial-state)
    (let ((best-solution NIL) (start-time (get-universal-time)) (result NIL) (start-run-time (get-internal-run-time)))
        (setf *is-expansions* 0)
        (setf *is-generations* 0)
        (loop while (< (- (get-universal-time) start-time) it-samp-max-time) do
            (setf result (it-samp-probe initial-state))
            (if (is-result-better best-solution result)
                (setf best-solution result)))
        (list (list best-solution) (- (get-internal-run-time) start-run-time) *is-expansions* *is-generations*)))


;;==============================================================================
;; ILDS - https://www.aaai.org/Papers/AAAI/1996/AAAI96-043.pdf Figure 3
;;==============================================================================

(defvar *ilds-generations*)
(defvar *ilds-expansions*)


(defun ilds-search (node heuristic k depth successors-f start-time)
    (incf *ilds-expansions*)
    (if (objective? (search-node-state node)) (return-from ilds-search node))
    (if (<= depth k) (return-from ilds-search NIL))
    (let ((state-successors (sort (funcall successors-f (search-node-state node)) #'< :key heuristic)) (k-left k) (best))
        (incf *ilds-generations* (length state-successors))
        ;; (format t "~D ~D~%" *ilds-expansions* *ilds-generations*)
        (setf state-successors (subseq state-successors 0 (min (1+ k) (length state-successors))))
        (loop while (and state-successors (>= k-left 0)) do
            (let ((state-successor (first state-successors)) (successor-node))
                (setf successor-node (ilds-search
                    (make-search-node :state state-successor :predecessor node)
                    heuristic k-left (1- depth) successors-f start-time))
                (if (and successor-node (or (not best)
                    (< (afectacao-state-total-cost state-successor) (afectacao-state-total-cost (search-node-state best)))))
                    (setf best successor-node))
                (if (>= (- (get-universal-time) start-time) ilds-max-time)
                    (return-from ilds-search best))
                (decf k-left)
                (setf state-successors (rest state-successors))))
        best))


(defun ilds (initial-state &key (add-costs T) (successors-f #'successors-best))
    (flet ((heuristic-wrapper (state)
        (if add-costs
            (+ (afectacao-state-total-cost state) (afectacao-state-heuristic state))
            (afectacao-state-heuristic state))))
        
        (let ((start-time (get-universal-time)) (start-time-internal (get-internal-run-time))
            (result) (solution) (k 0) (start-node (make-search-node :state initial-state :predecessor NIL)))
            (setf *ilds-generations* 0)
            (setf *ilds-expansions* 0)
            (loop while (< (- (get-universal-time) start-time) ilds-max-time) do
                (setf result (ilds-search start-node (lambda (state) (heuristic-wrapper state))
                    k (array-dimension (afectacao-state-problem initial-state) 0) successors-f start-time))
                (if (and result (or (not solution)
                    (< (afectacao-state-total-cost (search-node-state result)) (afectacao-state-total-cost (search-node-state solution)))))
                    (setf solution result))
                (incf k))
            (list (get-solution-list solution) (- (get-internal-run-time) start-time-internal) *ilds-expansions* *ilds-generations*))))


;;==============================================================================
;; RBFS - AIMA 3rd ed Figure 3.26
;;==============================================================================

(defvar *rbfs-generations*)
(defvar *rbfs-expansions*)


(defun rbfs-search (node f-limit successors-f)
    (incf *rbfs-expansions*)
    (if (objective? (search-node-state node)) (return-from rbfs-search (list node f-limit)))
    
    (let ((node-successors (sort (loop for state in (funcall successors-f (search-node-state node))
        collect (make-search-node :state state :predecessor node
        :f (max (+ (afectacao-state-total-cost state) (afectacao-state-heuristic state)) (search-node-f node))))
        #'< :key #'search-node-f)) (best) (alternative-f) (result))
        
        (incf *rbfs-generations* (length node-successors))
        ;; (format t "~D ~D~%" *rbfs-expansions* *rbfs-generations*)
        (loop while T do
            (setf best (first node-successors))
            (if (> (search-node-f best) f-limit)
                (return-from rbfs-search (list NIL (search-node-f best))))
            (if (second node-successors)
                (setf alternative-f (search-node-f (second node-successors)))
                (setf alternative-f MOST-POSITIVE-FIXNUM))
            (setf result (rbfs-search (first node-successors) (min f-limit alternative-f) successors-f))
            (if (first result) (return-from rbfs-search result))
            (setf (search-node-f best) (second result))
            (setf node-successors (sort node-successors #'< :key #'search-node-f)))))


(defun rbfs (initial-state &key (successors-f #'successors-best))
    (let ((start-time (get-internal-run-time)) (solution) (start-node (make-search-node :state initial-state :predecessor NIL
        :f (+ (afectacao-state-total-cost initial-state) (afectacao-state-heuristic initial-state)))))
        (setf *rbfs-generations* 0)
        (setf *rbfs-expansions* 0)
        (setf solution (rbfs-search start-node MOST-POSITIVE-FIXNUM successors-f))
        (list (get-solution-list (first solution)) (- (get-internal-run-time) start-time) *rbfs-expansions* *rbfs-generations*)))


;;##############################################################################
;;==============================================================================
;; MAIN FUNCTION & AUXILIARS
;;==============================================================================
;;##############################################################################

;;==============================================================================
;; MAIN FUNCTION AUXILIARS
;;==============================================================================

(defun get-external-shifts-from-state (state problem)
    (let ((external-problem (make-array (array-dimension (afectacao-state-problem state) 0)
        :element-type 'cons :initial-contents problem)) (shifts))
        (dolist (shift (afectacao-state-allocated state))
            (let ((new-shift NIL))
                (dolist (taskn shift)
                    (setf new-shift (cons (aref external-problem taskn) new-shift)))
                (setf shifts (cons new-shift shifts))))
        shifts))


(defun get-solution-external-representation (solution problem)
    (if (first solution)
        (get-external-shifts-from-state (first (last (first solution))) problem)
        NIL))


(defun create-problem-array-from-list (problem)
    (let ((problem-array (make-array (list (length problem) 4) :element-type 'number)) (i 0))
        (dolist (task problem)
            (let ((j 0))
                (dolist (attribute task)
                    (if (< j 2)
                        (setf (aref problem-array i j) (parse-integer (string-left-trim "L" (string attribute))))
                        (setf (aref problem-array i j) attribute))
                    (incf j)))
            (incf i))
        problem-array))


;;==============================================================================
;; MAIN FUNCTION
;;==============================================================================

(defun faz-afectacao (problem strategy &key stats)
    (let ((sorted-problem (sort (copy-list problem) #'< :key #'third))
        (array-problem) (initial-state) (procura-problem) (solution))
        
        (setf array-problem (create-problem-array-from-list sorted-problem))
        (setf initial-state (make-afectacao-state
            :problem array-problem
            :non-allocated (loop for n from 0 below (array-dimension array-problem 0) by 1 collect n)
            :cost 0 :total-cost 0 :heuristic 0 :heuristic-function #'greedy-allocation-heuristic :hash 0))
        (setf procura-problem (cria-problema initial-state '(successors-best)
            :objectivo? #'objective? :custo #'afectacao-state-cost
            :heuristica #'afectacao-state-heuristic
            :hash #'afectacao-state-hash :estado= #'state=))
        
        (if (> (array-dimension array-problem 0) 512)
            (setf (problema-operadores procura-problem) (list (lambda (state) (successors-best state :fraction (/ 1.0 32.0))))))
        
        (cond
            ((string-equal strategy "a*.melhor.heuristica")
                (setf solution (procura procura-problem "a*")))
            ((string-equal strategy "a*.melhor.heuristica.alternativa")
                (setf (afectacao-state-heuristic-function initial-state) #'fast-heuristic)
                (setf solution (procura procura-problem "a*")))
            ((string-equal strategy "sondagem.iterativa")
                (setf (afectacao-state-heuristic-function initial-state) (always 0))
                (setf solution (it-samp initial-state)))
            ((string-equal strategy "ILDS")
                (setf solution (ilds initial-state)))
            ((string-equal strategy "abordagem.alternativa")
                (setf solution (rbfs initial-state)))
            ((string-equal strategy "melhor.abordagem")
                (cond
                    ((<= (array-dimension array-problem 0) 192) ;; Up to problem 2
                        (setf solution (procura procura-problem "a*")))
                    ((<= (array-dimension array-problem 0) 256) ;; Up to problem 3
                        (setf solution (ilds initial-state)))
                    ((<= (array-dimension array-problem 0) 512) ;; Up to problem 4
                        (setf (afectacao-state-heuristic-function initial-state) #'fast-heuristic)
                        (setf solution (procura procura-problem "a*")))
                    ((<= (array-dimension array-problem 0) 768) ;; Up to problem 5
                        (setf (afectacao-state-heuristic-function initial-state) (lambda (state) (fast-heuristic state :multiplier 3)))
                        (setf solution (ilds initial-state :successors-f (lambda (state) (successors-best state :fraction (/ 1.0 32.0))))))
                    (T
                        (setf (afectacao-state-heuristic-function initial-state) (always 0))
                        (setf solution (it-samp initial-state)))))
            (T
                (if (or (string-equal strategy "profundidade") (string-equal strategy "largura") (string-equal strategy "profundidade-iterativa"))
                    (progn
                        (setf (afectacao-state-heuristic-function initial-state) (always 0))
                        (setf (problema-operadores procura-problem) '(successors))))
                (setf solution (procura procura-problem strategy :profundidade-maxima (array-dimension array-problem 0)))))
        
        (if stats
            (progn
                (print (rest solution))
                (format t "~%Total cost: ~D~%" (afectacao-state-total-cost (first (last (first solution)))))))
        (get-solution-external-representation solution sorted-problem)))
